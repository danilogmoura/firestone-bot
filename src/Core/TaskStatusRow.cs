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

    /// <summary>Disabled, Notification, Ready or Waiting — precedence defined in BotManager.</summary>
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
