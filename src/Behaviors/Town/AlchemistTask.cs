using System;
using System.Collections;
using System.Linq;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Alchemist;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using MelonLoader;
using UnityEngine;

namespace Firebot.Behaviors.Town;

public class AlchemistTask : BotTask
{
    private MelonPreferences_Entry<string> _resourceType;
    protected override string NotificationPath => Paths.BattleLoc.NotificationsLoc.ExperimentsBtn;

    protected override void OnConfigure(MelonPreferences_Category category)
    {
        if (_resourceType != null) return;

        _resourceType = category.CreateEntry(
            "resource_type",
            "",
            "Experiment Resources",
            $"ALCHEMIST EXPERIMENT RESOURCE CONFIGURATION. " +
            $"\nThis setting controls which experiment resources are used. " +
            $"\nValid IDs: 0=Dragon blood, 1=Strange dust, 2=Exotic coin. " +
            $"\nEnter comma-separated IDs (e.g. '0,1,2'). " +
            $"\nAny value other than 0, 1, or 2 will be ignored. " +
            $"\nDefault: empty (no resources, bot selects any available resource). " +
            $"\nEXAMPLES: '0,1' = Use Dragon blood and Strange dust. '2' = Only use Exotic coin."
        );
    }

    private string[] GetResourceTypes()
    {
        if (_resourceType == null)
        {
            Debug("[INFO] Missing resource_type entry. No resources set.");
            return Array.Empty<string>();
        }

        var value = _resourceType.Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            Debug("[INFO] Empty resource_type value. No resources set.");
            return Array.Empty<string>();
        }

        var validIds = new[] { "0", "1", "2" };
        var resources = value.Split(',')
            .Select(x => x.Trim())
            .Where(x => validIds.Contains(x))
            .ToArray();

        if (resources.Length != 0) return resources;

        Debug($"[FAILED] Invalid resource_type '{value}'. All values must be 0, 1, or 2. No resources set.");
        return Array.Empty<string>();
    }

    public override IEnumerator Execute()
    {
        yield return Notifications.Experiments;

        yield return new WaitForSeconds(10);
        var resources = GetResourceTypes();
        var experiments = new Experiments();
        yield return experiments.Claim(resources);
        yield return new WaitForSeconds(5);
        yield return experiments.Start(resources);
        NextRunTime = experiments.NextRunTime(resources);
        yield return Alchemist.Close;
    }
}