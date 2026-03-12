using System.Collections;
using System.Collections.Generic;
using Firebot.Core;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using Firebot.Utilities;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.BotActions;

public static class AutoResourceDrop
{
    private const float LoopThrottleSeconds = 0.1f;
    private const float IdlePollSeconds = 0.5f;
    private static readonly WaitForSeconds LoopThrottleWait = new(LoopThrottleSeconds);
    private static readonly WaitForSeconds IdlePollWait = new(IdlePollSeconds);

    private static readonly ResourceDropTarget[] Targets =
    {
        new("meteorite", new MeteoriteHunterBtn()),
        new("meteorite", new CoworkerMeteoriteHunterBtn()),
        new("beer", new DragonWithBeerBtn()),
        new("beer", new FemaleDragonWithBeerBtn())
    };

    private static bool _isRunning;
    private static object _autoResourceDropHandle;
    private static bool _isInitialized;
    private static MelonPreferences_Entry<bool> _isEnabled;
    private static readonly Dictionary<string, int> ClickCounts = new();

    private static bool IsEnabled => _isEnabled?.Value ?? false;

    public static void Initialize()
    {
        if (_isInitialized) return;

        var clazzName = StringUtils.Humanize(nameof(AutoResourceDrop));
        var sectionId = clazzName.Replace(" ", "_").ToLowerInvariant();

        var section = MelonPreferences.CreateCategory(sectionId, $"{clazzName} Settings");
        section.SetFilePath(BotSettings.ConfigPath);

        _isEnabled = section.CreateEntry(
            "enabled",
            false,
            $"Enable {clazzName}",
            $"Enables or disables the {clazzName} automation task. When disabled, this task will be ignored during the execution loop. Default: false."
        );

        section.SaveToFile();
        _isInitialized = true;
        Logger.Info($"{clazzName} configuration initialized.");
    }


    public static void Update()
    {
        if (IsEnabled)
        {
            if (!_isRunning) Start();
            return;
        }

        if (_isRunning) Stop();
    }

    private static void Start()
    {
        if (!IsEnabled) return;
        if (_isRunning) return;
        foreach (var target in Targets) target.WasVisible = false;
        ClickCounts.Clear();
        _isRunning = true;
        _autoResourceDropHandle = MelonCoroutines.Start(Loop());
        Logger.Info($"{nameof(AutoResourceDrop)} started.");
    }

    private static void Stop()
    {
        if (!_isRunning) return;
        _isRunning = false;
        if (_autoResourceDropHandle != null) MelonCoroutines.Stop(_autoResourceDropHandle);
        _autoResourceDropHandle = null;
        foreach (var target in Targets) target.WasVisible = false;
        Logger.Info($"{nameof(AutoResourceDrop)} stopped.");
    }

    private static IEnumerator Loop()
    {
        while (_isRunning)
        {
            var clickedAny = false;

            foreach (var target in Targets)
            {
                if (!_isRunning) yield break;
                var isVisibleNow = target.Button.IsVisible();
                var shouldClick = isVisibleNow && !target.WasVisible;
                target.WasVisible = isVisibleNow;

                if (!shouldClick) continue;

                yield return target.Button.Click(BotSettings.InteractionDelay);

                ClickCounts.TryAdd(target.ResourceId, 0);
                ClickCounts[target.ResourceId]++;

                var timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                Logger.Info($"[{timestamp}] Resource drop collected: {target.ResourceId} (count: {ClickCounts[target.ResourceId]})");
                clickedAny = true;
            }

            yield return clickedAny ? LoopThrottleWait : IdlePollWait;
        }
    }

    private sealed class ResourceDropTarget
    {
        public ResourceDropTarget(string resourceId, CachedGameButton button)
        {
            ResourceId = resourceId;
            Button = button;
        }

        public string ResourceId { get; }

        public CachedGameButton Button { get; }

        public bool WasVisible { get; set; }
    }

    private class MeteoriteHunterBtn : CachedGameButton
    {
        public MeteoriteHunterBtn()
            : base(Paths.BattleLoc.BattleResourceDropLoc.MeteoriteHunterBtn) { }

        public override bool IsVisible() =>
            new GameElement(Paths.BattleLoc.BattleResourceDropLoc.MeteoriteHunterHunter, this).IsVisible();
    }

    private class CoworkerMeteoriteHunterBtn : CachedGameButton
    {
        public CoworkerMeteoriteHunterBtn()
            : base(Paths.BattleLoc.BattleResourceDropLoc.CoworkerMeteoriteHunterBtn) { }

        public override bool IsVisible() =>
            new GameElement(Paths.BattleLoc.BattleResourceDropLoc.CoworkerMeteoriteHunterHunter, this).IsVisible();
    }

    private class DragonWithBeerBtn : CachedGameButton
    {
        public DragonWithBeerBtn()
            : base(Paths.BattleLoc.BattleResourceDropLoc.DragonWithBeerBtn) { }

        public override bool IsVisible() =>
            new GameElement(Paths.BattleLoc.BattleResourceDropLoc.DragonWithBeerHunter, this).IsVisible();
    }

    private class FemaleDragonWithBeerBtn : CachedGameButton
    {
        public FemaleDragonWithBeerBtn()
            : base(Paths.BattleLoc.BattleResourceDropLoc.FemaleDragonWithBeerBtn) { }

        public override bool IsVisible() =>
            new GameElement(Paths.BattleLoc.BattleResourceDropLoc.FemaleDragonWithBeerHunter, this).IsVisible();
    }
}