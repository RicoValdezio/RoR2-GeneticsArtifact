﻿using BepInEx;
using R2API;
using R2API.Utils;
using System.Reflection;
using UnityEngine;

namespace GeneticsArtifact
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(new string[] { "ResourcesAPI", "LanguageAPI" })]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class GeneticsArtifactPlugin : BaseUnityPlugin
    {
        private const string ModVer = "0.0.1";
        private const string ModName = "ArtifactOfGenetics";
        private const string ModGuid = "com.RicoValdezio.ArtifactOfGenetics";
        public static GeneticsArtifactPlugin Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            //RegisterAssetBundleProvider();
            ArtifactOfGenetics.Init();
        }

        private static void RegisterAssetBundleProvider()
        {
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GeneticsArtifact.genetics"))
            {
                AssetBundle bundle = AssetBundle.LoadFromStream(stream);
                AssetBundleResourcesProvider provider = new AssetBundleResourcesProvider("@Genetics", bundle);
                ResourcesAPI.AddProvider(provider);
            }
        }
    }
}
