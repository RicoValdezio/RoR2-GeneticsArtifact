using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using System.Reflection;
using UnityEngine;

namespace GeneticsArtifact
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(nameof(ContentAddition), nameof(ItemAPI), nameof(LanguageAPI), nameof(RecalculateStatsAPI), nameof(ArtifactCodeAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class GeneticsArtifactPlugin : BaseUnityPlugin
    {
        public const string ModVer = "4.3.0";
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

            ArtifactOfGenetics.Init();
            GeneTokens.Init();
            GeneTokenCalc.RegisterHooks();
            GeneEngineDriver.RegisterHooks();
        }
    }
}
