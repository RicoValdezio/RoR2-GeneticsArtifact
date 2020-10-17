using BepInEx.Configuration;

namespace GeneticsArtifact
{
    internal class ConfigMaster
    {
        internal static void Init()
        {
            GeneticMasterController.maxTrackers = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Master Settings", "Maximum General Purpose Trackers"), 4, new ConfigDescription("If the Tracker Per Monster ID flag is false, this is the maximum number of genetic families that will exist during a run.")).Value;
            GeneticMasterController.trackerPerMonsterID = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Master Settings", "Tracker Per Monster ID"), false, new ConfigDescription("If set to true, this will create a genetic family for each monster type.", new AcceptableValueList<bool>(true, false))).Value;
            GeneticMasterController.timeBetweenUpdates = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Master Settings", "Tracker Update Rate"), 10f, new ConfigDescription("The number of seconds between genetic family updates. Increase this if you have performance issues.")).Value;
            
            GeneTracker.absoluteCeil = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Tracker Settings", "Maximum Mutation Multiplier"), 5f, new ConfigDescription("The largest multiplier that can be applied to any particular stat. The minimum is determined by it's reciprocal.")).Value;
            GeneTracker.useSizeModifier = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Tracker Settings", "Enable Size Modifer"), false, new ConfigDescription("If set to true, size will be tracked and modified. (Note: this is potentially bugged for clients.)")).Value;
            GeneTracker.deviationFromParent = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Tracker Settings", "Maximum Deviation Percent"), 0.1f, new ConfigDescription("The maximum decimal amount that any stat can deviate from its master. At 0.1, a stat can be up to 10% different from the master that spawned it.")).Value;
            
            GeneticMasterController.applyToNeutrals = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Master Optional Settings", "Apply Artifact to Neutrals"), false, new ConfigDescription("If set to true, this will apply the artifact to neutral entities.", new AcceptableValueList<bool>(true, false))).Value;
            GeneticMasterController.applyToMinions = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Master Optional Settings", "Apply Artifact to Player Minions"), false, new ConfigDescription("If set to true, this will apply the artifact to player minions like turrets and drones.", new AcceptableValueList<bool>(true, false))).Value;
            
            GeneTracker.balanceLimit = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Balance Settings", "Maximum Mutation Product"), 10f, new ConfigDescription("The maximum product of the multipliers that can be applied to a monster compared to base. At 1, the multipliers will be evenly balanced. At 5, the multipliers will have a product of up to 5.")).Value;
            GeneTracker.balanceStep = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Balance Settings", "Balance Penalty Step Size"), 0.1f, new ConfigDescription("The penalty step applied when using the new balance system. Keep this at 0.1 unless you know what you're doing.")).Value;

            GeneBehaviour.accidentalDeathLogging = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Logging Settings", "Accidental Death Logging"), true, new ConfigDescription("If set to true, accidental deaths (the -health bug) will be logged as warnings.", new AcceptableValueList<bool>(true, false))).Value;
            GeneBehaviour.spawnLogging = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Logging Settings", "Spawn Logging"), false, new ConfigDescription("If set to true, all spawns will be logged with their genes. This is intense, so probably keep this false.", new AcceptableValueList<bool>(true, false))).Value;
            GeneticMasterController.statusLogging = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Logging Settings", "Master Status Logging"), false, new ConfigDescription("If set to true, all masters will log their genes periodically. This can be intense, so probably keep this false.", new AcceptableValueList<bool>(true, false))).Value;
            GeneticMasterController.timeBetweenStatusLogging = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("Logging Settings", "Status Logging Interval"), 300f, new ConfigDescription("The number of seconds between master status logs. At 300 it will log every 5 minutes.")).Value;
        }
    }
}