using System.Collections;
using System.Linq;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Library.FirestoneResearch;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using MelonLoader;

namespace Firebot.Behaviors.Town;

public class FirestoneResearchTask : BotTask
{
    private MelonPreferences_Entry<string> _researchPriority;
    public override string NotificationPath => Paths.BattleLoc.NotificationsLoc.FirestoneResearchBtn;

    public override IEnumerator Execute()
    {
        yield return Notifications.FirestoneResearch;

        var panel = new ResearchPanel();
        yield return panel.Claim();

        if (!ResearchPanel.HasEmptySlot)
        {
            Debug("[INFO] No empty slots. Scheduling next run.");
            NextRunTime = panel.NextRunTime();
            yield break;
        }

        yield return RunSelection();
        NextRunTime = panel.NextRunTime();
    }

    private IEnumerator SelectWithPreview(Node node, int index)
    {
        yield return node.Select(index);
        yield return TryStartPreview();
    }

    private IEnumerator RunSelection()
    {
        var priority = GetResearchPriority();
        var node = new Node();

        Debug(priority is { Length: > 0 }
            ? $"[INFO] Priority list loaded: {string.Join(",", priority)}"
            : "[INFO] No priority list. Using fallback selection.");

        while (ResearchPanel.HasEmptySlot)
        {
            var started = false;

            if (priority is { Length: > 0 })
                foreach (var priorityIndex in priority)
                    if (int.TryParse(priorityIndex, out var index))
                    {
                        yield return SelectWithPreview(node, index);

                        if (ResearchPanel.HasEmptySlot) continue;

                        started = true;
                        break;
                    }

            if (!started && ResearchPanel.HasEmptySlot)
            {
                yield return SelectAnyWithPreview(node);

                if (ResearchPanel.HasEmptySlot) break;
            }
        }
    }

    private IEnumerator SelectAnyWithPreview(Node node)
    {
        yield return node.SelectAny();
        yield return TryStartPreview();
    }

    private IEnumerator TryStartPreview()
    {
        if (Preview.IsUnlocked && !Preview.IsMaxed) yield return Preview.Start;
    }

    protected override void OnConfigure(MelonPreferences_Category category)
    {
        if (_researchPriority != null) return;

        _researchPriority = category.CreateEntry(
            "research_priority",
            "",
            "Research Priority",
            "FIRESTONE RESEARCH TALENT TREE PRIORITY CONFIGURATION. " +
            "This setting controls which talents are researched first based on their tree position. " +
            "TALENT IDs ARE ASSIGNED BY INDEX (ordered top to bottom, left to right within each tree screen). " +
            "Valid IDs for user input range from 1 to 16. " +
            "TREE I EXAMPLE - ID 1=Attribute damage, ID 2=Attribute health, ID 3=Attribute armor, ID 4=Fist fight, ID 5=Guardian power, ID 6=Projectiles, " +
            "ID 7=Raining gold, ID 8=Critical loot Bonus, ID 9=Critical loot Chance, ID 10=Weaklings, ID 11=Expose Weakness, " +
            "ID 12=Medal of honor, ID 13=Firestone Finder, ID 14=Trainer Skills, ID 15=Skip wave, ID 16=Expeditioner. " +
            "HOW TO USE: Enter comma-separated IDs in priority order (integers between 1-16). The bot will research talents in the exact sequence provided. " +
            "If a priority talent is unavailable (locked/completed), the bot will try the next priority. " +
            "If all priorities are unavailable or if this field is empty, the bot will select any available talent automatically. " +
            "EXAMPLES: '2,1,4' = Research Attribute health first, then Attribute damage, then Fist fight. " +
            "'7,8,9' = Research Raining gold first, then Critical loot Bonus, then Critical loot Chance. " +
            "'13' = Only research Firestone Finder, fallback to any available if completed. " +
            "Default: empty (no priority, bot selects any available talent)"
        );
    }

    private string[] GetResearchPriority()
    {
        if (_researchPriority == null)
        {
            Debug("[INFO] Missing research_priority entry. No priority set.");
            return null;
        }

        var value = _researchPriority.Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            Debug("[INFO] Empty research_priority value. No priority set.");
            return null;
        }

        var priorities = value.Split(',')
            .Select(x => x.Trim())
            .Where(x => int.TryParse(x, out var id) && id >= 1 && id <= 16)
            .Select(x => int.Parse(x).ToString())
            .ToArray();

        if (priorities.Length == 0)
        {
            Debug(
                $"[FAILED] Invalid research_priority '{value}'. All values must be integers between 1-16. No priority set.");
            return null;
        }

        return priorities;
    }
}