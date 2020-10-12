﻿using BepInEx.Configuration;

namespace GeneticsArtifact
{
    internal class ConfigMaster
    {
        internal static void Init()
        {
            GeneticMasterController.maxTrackers = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Master Settings", "Maximum Trackers"), 1, new ConfigDescription("If the Tracker Per Type flag is false, this is the maximum number of genetic families that will exist during a run.")).Value;
            GeneticMasterController.trackerPerMonsterID = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Master Settings", "Tracker Per Type"), false, new ConfigDescription("If set to true, this will create a genetic family for each monster type.", new AcceptableValueList<bool>(true, false))).Value;
            GeneticMasterController.timeBetweenUpdates = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Master Settings", "Tracker Update Rate"), 10f, new ConfigDescription("The number of seconds between genetic family updates. Increase this if you have performance issues.")).Value;
            GeneTracker.absoluteCeil = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Tracker Settings", "Maximum Mutation Multiplier"), 5f, new ConfigDescription("The largest multiplier that can be applied to any particular stat. The minimum is determined by it's reciprocal.")).Value;
            //GeneTracker.useBalancePenalty = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Tracker Settings", "Balance Penalty Sytem"), true, new ConfigDescription("If set to true, health will be penalized for having too high of other stats.")).Value;
            GeneTracker.useSizeModifier = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Tracker Settings", "Enable Size Modifer"), false, new ConfigDescription("If set to true, size will be tracked and modified. (Note: this is potentially bugged for clients.)")).Value;
            GeneTracker.deviationFromParent = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Tracker Settings", "Maximum Deviation Rate"), 0.1f, new ConfigDescription("The maximum decimal amount that any stat can deviate from its master. At 0.1, a stat can be up to 10% different from the master that spawned it.")).Value;
            GeneticMasterController.applyToNeutrals = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Master Optional Settings", "Apply Artifact to Neutrals"), false, new ConfigDescription("If set to true, this will apply the artifact to neutral entities.", new AcceptableValueList<bool>(true, false))).Value;
            GeneticMasterController.applyToMinions = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Master Optional Settings", "Apply Artifact to Player Minions"), false, new ConfigDescription("If set to true, this will apply the artifact to player minions like turrets and drones.", new AcceptableValueList<bool>(true, false))).Value;
            GeneTracker.balanceLimit = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Balance Settings", "Maximum Mutation Product"), 10f, new ConfigDescription("The maximum average multiplier that can be applied to a monster compared to base. I suggest setting this relatively low, as at 10 a monster can be a total of 10 times strong than its base version.")).Value;
            GeneTracker.balanceStep = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Balance Settings", "Balance Penalty Step Size"), 0.1f, new ConfigDescription("The penalty step applied when using the new balance system. Keep this at 0.1 unless you know what you're doing.")).Value;
        }
    }
}