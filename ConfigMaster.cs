using BepInEx.Configuration;

namespace GeneticsArtifact
{
    public class ConfigMaster
    {
        #region MasterController
        public static int maxTrackers;
        public static bool trackerPerMonsterID, applyToNeutrals, applyToMinions, statusLogging;
        public static float timeBetweenUpdates, timeBetweenStatusLogging;
        public static bool enableMasterPause;
        public static string rapidMutationType;
        #endregion

        #region GeneTracker
        public static bool useSizeModifier;
        public static float deviationFromParent, balanceLimit, balanceStep;
        #endregion

        #region GeneBehaviour
        public static bool accidentalDeathLogging, spawnLogging;
        #endregion

        #region GenePair-Specific
        public static float healthMax, healthMin, regenMax, regenMin, moveSpeedMax, moveSpeedMin, damageMax, damageMin, attackSpeedMax, attackSpeedMin, armorMax, armorMin, sizeMax, sizeMin;
        #endregion

        public static void Init()
        {
            maxTrackers = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("1. Master Settings", "Maximum General Purpose Trackers"), 4, new ConfigDescription("If the Tracker Per Monster ID flag is false, this is the maximum number of genetic families that will exist during a run.")).Value;
            trackerPerMonsterID = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("1. Master Settings", "Tracker Per Monster ID"), true, new ConfigDescription("If set to true, this will create a genetic family for each monster type.", new AcceptableValueList<bool>(true, false))).Value;
            timeBetweenUpdates = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("1. Master Settings", "Tracker Update Rate"), 10f, new ConfigDescription("The number of seconds between genetic family updates. Increase this if you have performance issues.")).Value;
            
            useSizeModifier = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("2. Tracker Settings", "Enable Size Modifer"), false, new ConfigDescription("If set to true, size will be tracked and modified. (Note: this is potentially bugged for clients.)", new AcceptableValueList<bool>(true, false))).Value;
            deviationFromParent = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("2. Tracker Settings", "Maximum Deviation Percent"), 0.1f, new ConfigDescription("The maximum decimal amount that any stat can deviate from its master. At 0.1, a stat can be up to 10% different from the master that spawned it.")).Value;
            
            applyToNeutrals = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("5. Master Optional Settings", "Apply Artifact to Neutrals"), false, new ConfigDescription("If set to true, this will apply the artifact to neutral entities.", new AcceptableValueList<bool>(true, false))).Value;
            applyToMinions = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("5. Master Optional Settings", "Apply Artifact to Player Minions"), false, new ConfigDescription("If set to true, this will apply the artifact to player minions like turrets and drones.", new AcceptableValueList<bool>(true, false))).Value;
            
            balanceLimit = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("3. Balance Settings", "Maximum Mutation Product"), 10f, new ConfigDescription("The maximum product of the multipliers that can be applied to a monster compared to base. At 1, the multipliers will be evenly balanced. At 5, the multipliers will have a product of up to 5.")).Value;
            balanceStep = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("3. Balance Settings", "Balance Penalty Step Size"), 0.1f, new ConfigDescription("The penalty step applied when using the new balance system. Keep this at 0.1 unless you know what you're doing.")).Value;

            accidentalDeathLogging = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("6. Logging Settings", "Accidental Death Logging"), true, new ConfigDescription("If set to true, accidental deaths (the -health bug) will be logged as warnings.", new AcceptableValueList<bool>(true, false))).Value;
            spawnLogging = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("6. Logging Settings", "Spawn Logging"), false, new ConfigDescription("If set to true, all spawns will be logged with their genes. This is intense, so probably keep this false.", new AcceptableValueList<bool>(true, false))).Value;
            statusLogging = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("6. Logging Settings", "Master Status Logging"), false, new ConfigDescription("If set to true, all masters will log their genes periodically. This can be intense, so probably keep this false.", new AcceptableValueList<bool>(true, false))).Value;
            timeBetweenStatusLogging = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("6. Logging Settings", "Status Logging Interval"), 300f, new ConfigDescription("The number of seconds between master status logs. At 300 it will log every 5 minutes.")).Value;

            healthMax = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Maximum Health Multiplier"), 5f, new ConfigDescription("The largest possible multiplier that can be applied to Health.")).Value;
            healthMin = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Minimum Health Multiplier"), 0.2f, new ConfigDescription("The smallest possible multiplier that can be applied to Health.")).Value;
            regenMax = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Maximum Regen Multiplier"), 2f, new ConfigDescription("The largest possible multiplier that can be applied to Regen.")).Value;
            regenMin = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Minimum Regen Multiplier"), 0.5f, new ConfigDescription("The smallest possible multiplier that can be applied to Regen.")).Value;
            moveSpeedMax = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Maximum MoveSpeed Multiplier"), 2f, new ConfigDescription("The largest possible multiplier that can be applied to MoveSpeed.")).Value;
            moveSpeedMin = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Minimum MoveSpeed Multiplier"), 0.5f, new ConfigDescription("The smallest possible multiplier that can be applied to MoveSpeed.")).Value;
            damageMax = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Maximum Damage Multiplier"), 5f, new ConfigDescription("The largest possible multiplier that can be applied to Damage.")).Value;
            damageMin = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Minimum Damage Multiplier"), 0.2f, new ConfigDescription("The smallest possible multiplier that can be applied to Damage.")).Value;
            attackSpeedMax = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Maximum AttackSpeed Multiplier"), 5f, new ConfigDescription("The largest possible multiplier that can be applied to AttackSpeed.")).Value;
            attackSpeedMin = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Minimum AttackSpeed Multiplier"), 0.2f, new ConfigDescription("The smallest possible multiplier that can be applied to AttackSpeed.")).Value;
            armorMax = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Maximum Armor Multiplier"), 5f, new ConfigDescription("The largest possible multiplier that can be applied to Armor.")).Value;
            armorMin = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Minimum Armor Multiplier"), 0.2f, new ConfigDescription("The smallest possible multiplier that can be applied to Armor.")).Value;
            sizeMax = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Maximum Size Multiplier"), 2f, new ConfigDescription("The largest possible multiplier that can be applied to Size.")).Value;
            sizeMin = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("4. Gene Specific Settings", "Minimum Size Multiplier"), 0.5f, new ConfigDescription("The smallest possible multiplier that can be applied to Size.")).Value;
            
            enableMasterPause = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("1. Master Settings", "Enable Master Pause"), true, new ConfigDescription("If set to true, all masters will be paused if the artifact is disabled during a run. If set to false, the masters will be destroyed.", new AcceptableValueList<bool>(true, false))).Value;

            rapidMutationType = GeneticsArtifactPlugin.Instance.Config.Bind(new ConfigDefinition("5. Master Optional Settings", "Rapid Mutation Type"), "Never", new ConfigDescription("Determines if/when the rapid mutation mode is active.", new AcceptableValueList<string>("Never", "Always", "OnlyEvents", "OnlyMoon", "EventsAndMoon"))).Value;
        }
    }
}