using BepInEx;
using BepInEx.Logging;
using System.Reflection;
using UnityEngine;

namespace GeneticsArtifact
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    //Stripped the R2API network stuff, need to do that manually now
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
    }
}
