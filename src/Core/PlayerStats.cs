using System;
using System.Collections.Generic;
using Il2Cpp;

namespace Firebot.Core;

/// <summary>
///     The character level, read from the game's own data instead of from what the screen shows.
///     <para>
///         0 means "not read", never "level zero": the game is asked on every call, and while it cannot
///         answer — scene still loading, character not created — the last value read is returned, or 0 when
///         there has never been one. Callers that gate a feature have to tell those two cases apart, which
///         is what <see cref="TryGetCharacterLevel" /> is for: reading 0 as "too low" would lock every
///         feature on the first scan of the bot.
///     </para>
/// </summary>
public static class PlayerStats
{
    private static readonly HashSet<string> LoggedFailures = new();
    private static int _lastKnownLevel;

    /// <summary>Latest level read, or the last one seen while the game cannot answer. 0 when never read.</summary>
    public static int CharacterLevel
    {
        get
        {
            var level = ReadLevel();
            if (level > 0) _lastKnownLevel = level;

            return _lastKnownLevel;
        }
    }

    /// <summary>False while no level has ever been read. Not the same statement as "the level is 0".</summary>
    public static bool HasKnownLevel => _lastKnownLevel > 0;

    /// <summary>
    ///     Reads the level and says whether it is known. A caller must not fall back to 0 when this returns
    ///     false: no level is not a low level, it is a level that does not exist yet.
    /// </summary>
    public static bool TryGetCharacterLevel(out int level)
    {
        level = CharacterLevel;
        return HasKnownLevel;
    }

    /// <summary>0 when the game is not in a state where the level can be read.</summary>
    private static int ReadLevel()
    {
        try
        {
            if (!GameInitialize.HasGameLoaded()) return 0;

            var loader = GameInitialize.HandlerLoader;
            if (loader == null) return 0;

            var characterLevel = loader.CharacterLevel;
            if (characterLevel == null || !characterLevel.hasLoaded) return 0;

            return characterLevel.Level;
        }
        catch (Exception e)
        {
            // Swallowing this silently would leave every level-gated task waiting forever with nothing in
            // the log to explain it — the exact failure the try/catch was meant to survive. Once per
            // distinct failure: this runs on every scan, and the failure does not change while it lasts.
            FailOnce($"{e.GetType().Name}|{e.Message}", $"Level read failed: {e.GetType().Name} - {e.Message}");
            return 0;
        }
    }

    private static void FailOnce(string key, string message)
    {
        if (LoggedFailures.Add(key)) Logger.Warning($"[PlayerStats] {message}");
    }
}