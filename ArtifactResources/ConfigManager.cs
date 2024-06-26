﻿using BepInEx.Configuration;

namespace GeneticsArtifact
{
    public class ConfigManager
    {
        public static ConfigEntry<int> timeLimit, deathLimit, governorType;

        public static ConfigEntry<float> geneVarianceLimit, geneCap, geneFloor, geneProductLimit;

        public static ConfigEntry<bool> maintainIfDisabled, enableGeneLimitOverrides;

        public static ConfigEntry<string> geneLimitOverrides;

        public static void Init(ConfigFile configFile)
        {
            governorType = configFile.Bind<int>(new ConfigDefinition("GeneEngineDriver Variables", "Learning Governor Type"), 0, new ConfigDescription("How the algorithm decides when to learn: 0 - Default, 1 - Time Only, 2 - Death Count Only", new AcceptableValueRange<int>(0, 2)));
            timeLimit = configFile.Bind<int>(new ConfigDefinition("GeneEngineDriver Variables", "Time Limit"), 60, new ConfigDescription("How many seconds between learnings:", new AcceptableValueRange<int>(5, 300))); // 5 seconds to 5 minutes
            deathLimit = configFile.Bind<int>(new ConfigDefinition("GeneEngineDriver Variables", "Death Limit"), 40, new ConfigDescription("How many monster deaths between learnings:", new AcceptableValueRange<int>(10, 100)));
            maintainIfDisabled = configFile.Bind<bool>(new ConfigDefinition("GeneEngineDriver Variables", "Keep Mutations While Disabled"), false, new ConfigDescription("Should the stat mods still be applied if the artifact is disabled mid-run:", new AcceptableValueList<bool>(true, false)));

            geneCap = configFile.Bind<float>(new ConfigDefinition("Mutation Variables", "Gene Value Cap"), 10.00f, new ConfigDescription("Maximum multiplier for any stat:", new AcceptableValueRange<float>(1f, 50f)));
            geneFloor = configFile.Bind<float>(new ConfigDefinition("Mutation Variables", "Gene Value Floor"), 0.01f, new ConfigDescription("Minimum multiplier for any stat:", new AcceptableValueRange<float>(0.01f, 1f)));
            geneProductLimit = configFile.Bind<float>(new ConfigDefinition("Mutation Variables", "Gene Product Cap"), 1.5f, new ConfigDescription("Maximum product of all stat multipliers:", new AcceptableValueRange<float>(1f, 10f)));
            geneVarianceLimit = configFile.Bind<float>(new ConfigDefinition("Mutation Variables", "Gene Variation Limit"), 0.1f, new ConfigDescription("How much a monster can differ from it`s master as a percent: 0.1 is 10% (Bulwark will be 5x this)", new AcceptableValueRange<float>(0.01f, 1f)));

            enableGeneLimitOverrides = configFile.Bind<bool>(new ConfigDefinition("Mutation Override Variables", "Enable Mutation Overrides"), false, new ConfigDescription("Should the mutation overrides be applied, use with caution", new AcceptableValueList<bool>(true, false)));
            geneLimitOverrides = configFile.Bind<string>(new ConfigDefinition("Mutation Override Variables", "Gene Limit Overrides"), "MoveSpeed,0.5,2|InvalidName,0.8,NaN", new ConfigDescription("Format is as follows: GeneName1,Floor1,Cap1|GeneName2,Floor2,Cap2 where GeneName is in (MaxHealth,MoveSpeed,AttackSpeed,AttackDamage) and Floor and Cap are parseable numerics"));
        }
    }

    public enum GovernorType
    {
        Default,
        TimeOnly,
        DeathsOnly
    }
}
