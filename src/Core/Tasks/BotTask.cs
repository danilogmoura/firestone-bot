using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Firebot.GameModel.Base;
using MelonLoader;
using static Firebot.Utilities.StringUtils;

namespace Firebot.Core.Tasks;

public abstract class BotTask
{
    private readonly string _className;
    private MelonPreferences_Category _category;
    private MelonPreferences_Entry<bool> _enabledEntry;
    private GameElement _notificationElement;
    private bool _levelLockLogged;
    private bool _levelWaitLogged;

    protected BotTask()
    {
        _className = GetType().Name;
    }

    public string SectionTitle => Humanize(GetType().Name);

    public DateTime NextRunTime { get; protected set; } = DateTime.MinValue;

    /// <summary>
    ///     Character level the feature behind this task needs. <see cref="LevelRequirements.None" /> — the
    ///     default — means the feature is available from the start, and costs no game lookup.
    /// </summary>
    public virtual int MinimumLevel => LevelRequirements.None;

    public DateTime? LastRunTime { get; set; }

    protected virtual string NotificationPath => null;

    public bool IsEnabled
    {
        get => _enabledEntry?.Value ?? false;
        protected set => _enabledEntry.Value = value;
    }

    private GameElement NotificationElement
    {
        get
        {
            if (_notificationElement != null) return _notificationElement;
            if (string.IsNullOrEmpty(NotificationPath)) return null;

            _notificationElement = new GameElement(NotificationPath);
            return _notificationElement;
        }
    }

    public void InitializeConfig(string configPath)
    {
        if (_enabledEntry != null) return;

        var sectionId = SectionTitle.Replace(" ", "_").ToLowerInvariant();

        // The display name is what the panel shows; the .cfg stores only the identifier. So it carries no
        // "Settings" suffix — the window is already the bot's settings, and the section header is what
        // names the feature.
        _category = MelonPreferences.CreateCategory(sectionId, SectionTitle);
        _category.SetFilePath(configPath);

        // Named after the task rather than a generic "Enable Task": the row can then be read on its own,
        // without the section header above it.
        _enabledEntry = _category.CreateEntry("enabled", false, $"Enable {SectionTitle}",
            $"Enables or disables the {SectionTitle} automation task." +
            $"\nWhen disabled, this task will be ignored during the execution loop.");

        OnConfigure(_category);
        _category.SaveToFile();
    }

    protected virtual void OnConfigure(MelonPreferences_Category category) { }

    /// <summary>
    ///     True while the game does not offer this task's feature yet, or while the level is not known.
    ///     <para>
    ///         The unknown case is a lock on purpose, but only for a task that asked for a level: running the
    ///         task would click a menu that may not exist yet, which is what breaks the flow. A task with no
    ///         requirement returns on the first line and never looks at the level.
    ///     </para>
    ///     <para>
    ///         Nothing is written back to the settings. The user's "enabled" is theirs; this is a runtime
    ///         condition, and it clears itself the moment the level arrives — including for the level
    ///         requirement the character has not reached yet.
    ///     </para>
    /// </summary>
    public bool IsLevelLocked
    {
        get
        {
            if (MinimumLevel <= LevelRequirements.None) return false;

            if (!PlayerStats.TryGetCharacterLevel(out var level))
            {
                if (!_levelWaitLogged)
                {
                    _levelWaitLogged = true;
                    Debug($"Level not read yet; holding off (needs {MinimumLevel}).");
                }

                return true;
            }

            _levelWaitLogged = false;

            if (level >= MinimumLevel)
            {
                _levelLockLogged = false;
                return false;
            }

            if (!_levelLockLogged)
            {
                _levelLockLogged = true;
                Logger.Debug($"[{SectionTitle}] Locked until level {MinimumLevel}. Character is at {level}.");
            }

            return true;
        }
    }

    public bool IsReady()
        => !IsLevelLocked && (IsNotificationVisibleCore() || (IsEnabled && DateTime.Now >= NextRunTime));

    /// <summary>
    ///     The check the scheduler makes before it even looks at readiness: a popup can be clicked from here,
    ///     so the level guard has to be in front of it too.
    /// </summary>
    public bool IsNotificationVisible()
        => !IsLevelLocked && IsNotificationVisibleCore();

    /// <summary>The level is looked up once per call through the guards above, not once per branch.</summary>
    private bool IsNotificationVisibleCore()
        => IsEnabled && NotificationElement != null && NotificationElement.IsVisible();

    public abstract IEnumerator Execute();

    protected void Debug(string message, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        => Logger.Debug($"[{_className}::{member}:{line}] {message}");
}