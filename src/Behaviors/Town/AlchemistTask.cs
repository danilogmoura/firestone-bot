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

    public override int MinimumLevel => LevelRequirements.Alchemist;

    protected override void OnConfigure(MelonPreferences_Category category)
    {
        if (_resourceType != null) return;

        _resourceType = category.CreateEntry(
            "resource_type",
            "",
            "Resources",
            $"ALCHEMIST EXPERIMENT RESOURCE CONFIGURATION. " +
            $"\nThis setting controls which experiment resources are used. " +
            $"\nValid IDs: 0=Dragon blood, 1=Strange dust, 2=Exotic coin. " +
            $"\nEnter comma-separated IDs (e.g. '0,1,2'). " +
            $"\nAny value other than 0, 1, or 2 will be ignored. " +
            $"\nDefault: empty. If no value is provided, the task will be disabled." +
            $"\nEXAMPLES: '0,1' = Use Dragon blood and Strange dust. '2' = Only use Exotic coin."
        );
    }

    private string[] GetResourceTypes()
    {
        if (_resourceType == null || string.IsNullOrWhiteSpace(_resourceType.Value))
        {
            Debug("[INFO] Missing resource_type entry. No resources set. Task will be disabled.");
            IsEnabled = false;
            return Array.Empty<string>();
        }

        var value = _resourceType.Value;

        var resources = value.Split(',')
            .Select(x => x.Trim())
            .Where(x => Experiments.ValidResourceIds.Contains(x))
            .Distinct()
            .ToArray();

        if (resources.Length != 0) return resources;

        Debug($"[FAILED] Invalid resource_type '{value}'. All values must be 0, 1, or 2. No resources set.");
        return Array.Empty<string>();
    }

    public override IEnumerator Execute()
    {
        yield return Notifications.Experiments;

        yield return new WaitForSeconds(3);
        var experiments = new Experiments();
        var resources = GetResourceTypes();
        yield return experiments.Claim();

        var startableResources = resources
            .Where(resource =>
            {
                var quantity = experiments.ResourceQuantity(resource);
                if (quantity > 0) return true;

                Debug($"[INFO] Resource '{resource}' is unavailable ({quantity:0.##}). " +
                      "Skipping new experiment start.");
                return false;
            })
            .ToArray();

        if (startableResources.Length == 0)
        {
            NextRunTime = DateTime.Now.AddHours(1);
            Debug("[INFO] No configured resources are available for a new experiment. " +
                  "Retrying resource check in one hour.");
            yield return Alchemist.Close;
            yield break;
        }

        yield return new WaitForSeconds(1);
        yield return experiments.Start(startableResources);
        NextRunTime = experiments.NextRunTime(resources);
        yield return Alchemist.Close;
    }
}