using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticsArtifact
{
    public class ArtifactOfGenetics
    {
        public static ArtifactDef def;

        internal static void Init()
        {
            def = ScriptableObject.CreateInstance<ArtifactDef>();

            LanguageOverride.customLanguage.Add("GENE_NAME_TOKEN", "Artifact of Genetics");
            LanguageOverride.customLanguage.Add("GENE_DESC_TOKEN", "Monsters' stats will change based on the performance of previous monsters.");

            def.nameToken = "GENE_NAME_TOKEN";
            def.descriptionToken = "GENE_DESC_TOKEN";
            def.smallIconSelectedSprite = GeneticsArtifactPlugin.geneticAssetBundle.LoadAsset<Sprite>("Assets/Genetics/Selected.png");
            def.smallIconDeselectedSprite = GeneticsArtifactPlugin.geneticAssetBundle.LoadAsset<Sprite>("Assets/Genetics/Unselected.png");

            On.RoR2.ContentManager.SetContentPacks += ContentManager_SetContentPacks;
        }

        private static void ContentManager_SetContentPacks(On.RoR2.ContentManager.orig_SetContentPacks orig, List<ContentPack> newContentPacks)
        {
            ContentPack pack = new ContentPack
            {
                artifactDefs = new List<ArtifactDef> { def }.ToArray()
            };

            newContentPacks.Add(pack);
            orig(newContentPacks);
        }
    }
}