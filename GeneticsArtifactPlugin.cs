using BepInEx;
using BepInEx.Logging;
using System.Reflection;
using UnityEngine;
using R2API.Utils;
using R2API;

namespace GeneticsArtifact
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ArtifactAPI), nameof(ItemAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class GeneticsArtifactPlugin : BaseUnityPlugin
    {
        public const string ModVer = "3.2.2";
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
        }
    }
}
