using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using System.Reflection;
using UnityEngine;

namespace GeneticsArtifact
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("Rein.RogueWisp", BepInDependency.DependencyFlags.SoftDependency)] //This is bad and I hate it that load order caused the bug
    [R2APISubmoduleDependency(new string[] { "ResourcesAPI", "LanguageAPI" })]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class GeneticsArtifactPlugin : BaseUnityPlugin
    {
        private const string ModVer = "2.6.0";
        private const string ModName = "Genetics";
        private const string ModGuid = "com.RicoValdezio.ArtifactOfGenetics";
        public static GeneticsArtifactPlugin Instance;
        internal static ManualLogSource geneticLogSource;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            geneticLogSource = Instance.Logger;
            RegisterAssetBundleProvider();
            ConfigMaster.Init();
            ArtifactOfGenetics.Init();
            GeneticMasterController.Init();
        }

        private static void RegisterAssetBundleProvider()
        {
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GeneticsArtifact.ArtifactResources.genetics"))
            {
                AssetBundle bundle = AssetBundle.LoadFromStream(stream);
                AssetBundleResourcesProvider provider = new AssetBundleResourcesProvider("@Genetics", bundle);
                ResourcesAPI.AddProvider(provider);
            }
        }

        private void OnDisable()
        {
            //GeneticMasterController.Cleanup();
        }
    }
}
