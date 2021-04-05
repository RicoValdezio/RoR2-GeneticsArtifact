using BepInEx;
using BepInEx.Logging;
using RoR2;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GeneticsArtifact
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class GeneticsArtifactPlugin : BaseUnityPlugin
    {
        private const string ModVer = "3.0.0";
        private const string ModName = "Genetics";
        private const string ModGuid = "com.RicoValdezio.ArtifactOfGenetics";
        public static GeneticsArtifactPlugin Instance;
        internal static ManualLogSource geneticLogSource;
        internal static AssetBundle geneticAssetBundle;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            geneticLogSource = Instance.Logger;
            geneticAssetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("GeneticsArtifact.ArtifactResources.genetics"));

            LanguageOverride.Init();
            ConfigMaster.Init();
            ArtifactOfGenetics.Init();
            GeneticMasterController.Init();
            NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(ModGuid + ":" + ModVer);
        }
    }
}
