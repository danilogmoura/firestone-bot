using System;

namespace Firebot.Core;

/// <summary>
///     One row of the task status table.
///     <para>
///         It carries data, not ready-made text. The console and the in-game screen need different
///         widths, but they have to agree on the <em>decision</em> — what the status is, in what order
///         the tasks appear, what is null. That part lives here, and only here.
///     </para>
/// </summary>
internal readonly struct TaskStatusRow
{
    public TaskStatusRow(string name, string status, string timeLeft, DateTime? nextRun, DateTime? lastRun)
    {
        Name = name;
        Status = status;
        TimeLeft = timeLeft;
        NextRun = nextRun;
        LastRun = lastRun;
    }

    public string Name { get; }

    /// <summary>
    ///     Cell text for "there is nothing here". One definition, because the console and the status screen
    ///     both print it and neither should drift.
    /// </summary>
    public const string NoValue = "-";

    // ---- Status names ----
    // Constants rather than literals: the manager decides the status and the status screen matches on it to
    // pick a colour. As literals, renaming one side silently downgraded the other to "idle".

    public const string Disabled = "Disabled";

    /// <summary>
    ///     The task has something waiting to be clicked in the game. The internal name for it is
    ///     "notification", which says nothing to someone reading the table.
    /// </summary>
    public const string Popup = "Popup";

    public const string Ready = "Ready";

    public const string Waiting = "Waiting";

    /// <summary>Disabled, Popup, Ready or Waiting — precedence defined in BotManager.</summary>
    public string Status { get; }

    /// <summary>
    ///     Duration already formatted. It stays a string on purpose: it depends on the instant the row
    ///     was built, so recomputing it in the consumer would introduce drift rather than solve it.
    /// </summary>
    public string TimeLeft { get; }

    /// <summary>Null when the task is disabled: there is no next run that matters.</summary>
    public DateTime? NextRun { get; }

    public DateTime? LastRun { get; }
}
