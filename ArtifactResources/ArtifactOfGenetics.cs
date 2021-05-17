using R2API;
using RoR2;
using UnityEngine;

namespace GeneticsArtifact
{
    public class ArtifactOfGenetics
    {
        public static ArtifactDef def;

        internal static void Init()
        {
            def = ScriptableObject.CreateInstance<ArtifactDef>();

            LanguageAPI.Add("GENETIC_ARTIFACT_NAME_TOKEN", "Artifact of Genetics");
            LanguageAPI.Add("GENETIC_ARTIFACT_DESCRIPTION_TOKEN", "Monsters will spawn with adjusted stats. Adjustments are determined by a genetic algorithm.");

            def.nameToken = "GENETIC_ARTIFACT_NAME_TOKEN";
            def.descriptionToken = "GENETIC_ARTIFACT_DESCRIPTION_TOKEN";
            def.smallIconSelectedSprite = GeneticsArtifactPlugin.geneticAssetBundle.LoadAsset<Sprite>("Assets/Genetics/Selected.png");
            def.smallIconDeselectedSprite = GeneticsArtifactPlugin.geneticAssetBundle.LoadAsset<Sprite>("Assets/Genetics/Unselected.png");

            ArtifactAPI.Add(def);
        }
    }
}