using System.Collections;
using System.Collections.Generic;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Firebot.Core;

public static class Watchdog
{
    private enum SweepGroup
    {
        Events,
        PopupsClose,
        PopupsCollect,
        MenusClose
    }

    private const int GroupCount = 4;

    /// <summary>
    ///     Passes over the nuisance roots. One pass closes what is open at that moment; a popup that only becomes
    ///     reachable once another one is closed needs the next pass.
    /// </summary>
    private const int Passes = 3;

    /// <summary>
    ///     Measurement harness for <see cref="ForceClearAll" />, reported under <c>Logger.IsDebugEnabled</c>.
    ///     <para>
    ///         The sweep is the only place in the mod that fans out over the hierarchy: every probe resolves a
    ///         full path twice (once to build it, once to test visibility) and each resolution searches the scene
    ///         by name. When nothing is open the sweep never yields, so all of it lands in the single frame that
    ///         called it — which is what made it worth counting before deciding whether to spread it over frames
    ///         or leave it alone.
    ///     </para>
    ///     <para>
    ///         Measured in game with all 10 tasks running: 3 to 27 probes per sweep, 0.2 to 0.7ms of main thread,
    ///         one frame each. The only outlier was the first sweep after a start — 13.7ms of first lookups and
    ///         warm-up — and the running average settled at 1.0ms. That is why the sweep was left as it was: three
    ///         passes cost between one and four percent of a 60 fps frame, and skipping the empty ones would save
    ///         about 0.3ms of the safety net that closes what the game leaves open.
    ///     </para>
    ///     <para>
    ///         The counters stay on outside debug mode — two increments and two timestamp reads per probe, against
    ///         a path resolution each. Delete this class together with the calls that feed it to take it out for
    ///         good.
    ///     </para>
    /// </summary>
    private sealed class SweepMeter
    {
        private const int SummaryEvery = 20;

        private static int _nextNumber = 1;
        private static int _sweeps;
        private static long _blockedTicksTotal;
        private static long _blockedTicksMax;
        private static long _probesTotal;

        private readonly string _phase;
        private readonly int _number;
        private readonly long _startedAt;
        private readonly int _startFrame;
        private readonly int[] _probes = new int[GroupCount];
        private readonly long[] _probeTicks = new long[GroupCount];
        private int _candidates;
        private int _closed;
        private long _candidateTicks;
        private long _maxProbeTicks;
        private long _waitedTicks;

        public SweepMeter(string phase)
        {
            _phase = phase;
            _number = _nextNumber++;
            _startedAt = Stopwatch.GetTimestamp();
            _startFrame = Time.frameCount;
        }

        /// <summary>One child probed, with the cost of building its path and testing whether it is visible.</summary>
        public void Probe(SweepGroup group, long ticks)
        {
            _probes[(int)group]++;
            _probeTicks[(int)group] += ticks;
            if (ticks > _maxProbeTicks) _maxProbeTicks = ticks;
        }

        /// <summary>A path that survived the probe, resolved again as a button to ask whether it is visible.</summary>
        public void Candidate(long ticks)
        {
            _candidates++;
            _candidateTicks += ticks;
        }

        public void Closing() => _closed++;

        /// <summary>How long a click took, so the wait can be told apart from the blocking.</summary>
        public void Waited(long ticks) => _waitedTicks += ticks;

        /// <summary>
        ///     "blocked" is the wall time minus what the sweep spent waiting on a click: that wait is the only
        ///     thing this coroutine yields on, so what remains is main-thread time — which is what the game pays
        ///     for in frames. "frames" says how many frames the sweep spanned, so a single-frame sweep of 100ms is
        ///     one hitch, while the same total spread over four frames is not.
        /// </summary>
        public void Report()
        {
            var wallTicks = Stopwatch.GetTimestamp() - _startedAt;
            var probes = 0;
            var probeTicks = 0L;

            for (var i = 0; i < GroupCount; i++)
            {
                probes += _probes[i];
                probeTicks += _probeTicks[i];
            }

            var blockedTicks = wallTicks - _waitedTicks;

            if (Logger.IsDebugEnabled)
                Logger.Info($"[Watchdog] #{_number} \"{_phase}\" | passes {Passes} | probes {probes} | " +
                            $"candidates {_candidates} | closed {_closed} | frames {Time.frameCount - _startFrame + 1} | " +
                            $"wall {Ms(wallTicks):0.0}ms | waited {Ms(_waitedTicks):0.0}ms | " +
                            $"blocked {Ms(blockedTicks):0.0}ms | probe cost {Ms(probeTicks):0.0}ms " +
                            $"(max {Ms(_maxProbeTicks):0.0}ms) | candidate cost {Ms(_candidateTicks):0.0}ms");

            if (Logger.IsDebugEnabled)
                Logger.Info($"[Watchdog] #{_number} by group | events {_probes[0]}/{Ms(_probeTicks[0]):0.0}ms | " +
                            $"popups-close {_probes[1]}/{Ms(_probeTicks[1]):0.0}ms | " +
                            $"popups-collect {_probes[2]}/{Ms(_probeTicks[2]):0.0}ms | " +
                            $"menus-close {_probes[3]}/{Ms(_probeTicks[3]):0.0}ms");

            _sweeps++;
            _probesTotal += probes;
            _blockedTicksTotal += blockedTicks;
            if (blockedTicks > _blockedTicksMax) _blockedTicksMax = blockedTicks;

            if (_sweeps % SummaryEvery != 0) return;
            if (!Logger.IsDebugEnabled) return;

            Logger.Info($"[Watchdog] summary | sweeps {_sweeps} | probes avg {_probesTotal / (double)_sweeps:0.0} | " +
                        $"blocked avg {Ms(_blockedTicksTotal / _sweeps):0.0}ms | " +
                        $"blocked max {Ms(_blockedTicksMax):0.0}ms");
        }

        private static double Ms(long ticks) => ticks * 1000.0 / Stopwatch.Frequency;
    }

    private static IEnumerable<string> EnumerateNuisancePaths(SweepMeter meter)
    {
        foreach (var path in EnumerateChildPaths(meter, SweepGroup.Events, new GameElement(Paths.Watchdog.EventsRoot),
                     Paths.Watchdog.EventsRoot,
                     Paths.Watchdog.CloseSuffix,
                     "bg/closeButton"))
            yield return path;

        foreach (var path in EnumerateChildPaths(meter, SweepGroup.PopupsClose,
                     new GameElement(Paths.Watchdog.PopupsRoot),
                     Paths.Watchdog.PopupsRoot,
                     Paths.Watchdog.CloseSuffix,
                     "bg/closeButton"))
            yield return path;

        foreach (var path in EnumerateChildPaths(meter, SweepGroup.PopupsCollect,
                     new GameElement(Paths.Watchdog.PopupsRoot),
                     Paths.Watchdog.PopupsRoot,
                     Paths.Watchdog.CollectSuffix,
                     "bg/collectButton"))
            yield return path;

        foreach (var path in EnumerateChildPaths(meter, SweepGroup.MenusClose,
                     new GameElement(Paths.Watchdog.MenusRoot),
                     Paths.Watchdog.MenusRoot,
                     Paths.Watchdog.MenuCloseSuffix,
                     "closeButton"))
            yield return path;
    }

    private static IEnumerable<string> EnumerateChildPaths(SweepMeter meter, SweepGroup group,
        GameElement rootElement, string basePath, string suffix, string probePath)
    {
        foreach (var child in rootElement.GetChildren())
        {
            var started = Stopwatch.GetTimestamp();
            var probe = new GameElement(probePath, child);
            var visible = probe.IsVisible();
            meter.Probe(group, Stopwatch.GetTimestamp() - started);

            if (visible)
                yield return $"{basePath}/{child.Name}{suffix}";
        }
    }

    /// <summary>
    ///     Closes whatever the game left open. <paramref name="phase" /> only labels the measurement line, so a
    ///     slow sweep can be told apart from a fast one in the log.
    /// </summary>
    public static IEnumerator ForceClearAll(string phase = null)
    {
        var meter = new SweepMeter(phase ?? "cleanup");

        for (var pass = 0; pass < Passes; pass++)
            foreach (var path in EnumerateNuisancePaths(meter))
            {
                var started = Stopwatch.GetTimestamp();
                var gameButton = new GameButton(path);
                var visible = gameButton.IsVisible();
                meter.Candidate(Stopwatch.GetTimestamp() - started);

                if (!visible) continue;

                Logger.Debug($"[Watchdog] Closing popup: {path}");
                meter.Closing();

                var beforeClick = Stopwatch.GetTimestamp();
                yield return gameButton.Click();
                meter.Waited(Stopwatch.GetTimestamp() - beforeClick);
            }

        meter.Report();
    }
}