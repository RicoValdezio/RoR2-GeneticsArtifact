using BepInEx;
using BepInEx.Logging;
using RoR2;
using RoR2.ContentManagement;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GeneticsArtifact
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency("PlasmaCore.CRCore3", BepInDependency.DependencyFlags.SoftDependency)]
    public class GeneticsArtifactPlugin : BaseUnityPlugin
    {
        private const string ModVer = "3.2.2";
        private const string ModName = "Genetics";
        internal const string ModGuid = "com.RicoValdezio.ArtifactOfGenetics";
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
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new GeneticsContentProvider());
        }
    }
}
