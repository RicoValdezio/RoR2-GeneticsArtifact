using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using R2API.Utils;
using System.Reflection;
using UnityEngine;

namespace GeneticsArtifact
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    #region [BepInDeps]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api" + ".artifactcode", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api" + ".content_management", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api" + ".items", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api" + ".language", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api" + ".recalculatestats", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    #endregion
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class GeneticsArtifactPlugin : BaseUnityPlugin
    {
        public const string ModVer = "4.5.3";
        public const string ModName = "Genetics";
        public const string ModGuid = "com.RicoValdezio.ArtifactOfGenetics";
        public static GeneticsArtifactPlugin Instance;
        public static ManualLogSource geneticLogSource;
        public static AssetBundle geneticAssetBundle;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            geneticLogSource = Instance.Logger;
            geneticAssetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("GeneticsArtifact.ArtifactResources.genetics"));

            ConfigManager.Init(Config);

            ArtifactOfGenetics.Init();
            GeneTokens.Init();
            GeneTokenCalc.RegisterHooks();
            GeneEngineDriver.RegisterHooks();
            
            foreach (PluginInfo plugin in Chainloader.PluginInfos.Values) { if (plugin.Metadata.GUID.Equals("com.rune580.riskofoptions")) { RiskOfOptionsCompat.Init(); break; } }
        }
    }
}
