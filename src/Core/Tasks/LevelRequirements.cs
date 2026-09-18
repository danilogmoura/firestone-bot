namespace Firebot.Core.Tasks;

/// <summary>
///     Character level each automation needs before the game offers the feature it drives.
///     <para>
///         One table instead of a number per task: the requirement belongs to the feature, and when the game
///         changes an unlock level this is the single line to edit. A task whose requirement is not known
///         stays at <see cref="None" /> rather than guessing — guessing is what makes a task click a menu
///         that does not exist yet.
///     </para>
/// </summary>
public static class LevelRequirements
{
    /// <summary>
    ///     No requirement: the feature exists from the start of the game. It is 1, the first level a
    ///     character can have, so "no requirement" and "level 1" are the same statement.
    /// </summary>
    public const int None = 1;

    public const int FreePickaxes = 50;

    public const int EngineerTools = 50;

    public const int WarfrontCampaign = 50;

    public const int Alchemist = 120;

    public const int Oracle = 200;
}
