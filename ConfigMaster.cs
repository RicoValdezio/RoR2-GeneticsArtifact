using BepInEx.Configuration;

namespace GeneticsArtifact
{
    internal class ConfigMaster
    {
        internal static void Init()
        {
            GeneticMasterController.maxTrackers = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("General Settings", "Maximum Trackers"), 1, new ConfigDescription("If the Tracker Per Type flag is false, this is the maximum number of genetic families that will exist during a run.")).Value;
            GeneticMasterController.trackerPerMonsterID = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("General Settings", "Tracker Per Type"), false, new ConfigDescription("If set to true, this will create a genetic family for each monster type.", new AcceptableValueList<bool>(true, false))).Value;
            GeneticMasterController.timeBetweenUpdates = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("General Settings", "Tracker Update Rate"), 10f, new ConfigDescription("The number of seconds between genetic family updates. Increase this if you have performance issues.")).Value;
            GeneTracker.absoluteCeil = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("General Settings", "Maximum Mutation"), 5f, new ConfigDescription("The largest multiplier that can be applied to any particular stat. The minimum is determined by it's reciprocal.")).Value;
            GeneTracker.useBalancePenalty = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("General Settings", "Balance Penalty Sytem"), true, new ConfigDescription("If set to true, health will be penalized for having too high of other stats.")).Value;
        }
    }
}