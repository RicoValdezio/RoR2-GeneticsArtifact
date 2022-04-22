using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
using BepInEx;

namespace GeneticsArtifact
{
    public class ConfigManager
    {
        public static ConfigEntry<int> timeLimit, deathLimit, governorType;

        public static void Init(ConfigFile configFile)
        {
            governorType = configFile.Bind<int>(new ConfigDefinition("GeneEngineDriver Variables", "Learning Governor Type"), 0, new ConfigDescription("How the algorithm decides when to learn: 0 - Default, 1 - Time Only, 2 - Death Count Only", new AcceptableValueRange<int>(0, 2)));
            timeLimit = configFile.Bind<int>(new ConfigDefinition("GeneEngineDriver Variables", "Time Limit"), 60, new ConfigDescription("How many seconds between learnings:", new AcceptableValueRange<int>(5, 300))); // 5 seconds to 5 minutes
            deathLimit = configFile.Bind<int>(new ConfigDefinition("GeneEngineDriver Variables", "Death Limit"), 40, new ConfigDescription("How many monster deaths between learnings:", new AcceptableValueRange<int>(10, 100)));
        }
    }

    public enum GovernorType
    {
        Default,
        TimeOnly,
        DeathsOnly
    }
}
