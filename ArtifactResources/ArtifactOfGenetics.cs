using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticsArtifact
{
    public class ArtifactOfGenetics
    {
        public static ArtifactDef artifactDef;
        public static ArtifactCode artifactCode;
        public static ArtifactCompoundDef geneArtifactCompoundDef;

        internal static void Init()
        {
            LanguageAPI.Add("GENETIC_ARTIFACT_NAME_TOKEN", "Artifact of Genetics");
            LanguageAPI.Add("GENETIC_ARTIFACT_DESCRIPTION_TOKEN", "Monsters will spawn with adjusted stats. Adjustments are determined by a genetic algorithm.");

            artifactDef = ScriptableObject.CreateInstance<ArtifactDef>();
            artifactDef.nameToken = "GENETIC_ARTIFACT_NAME_TOKEN";
            artifactDef.descriptionToken = "GENETIC_ARTIFACT_DESCRIPTION_TOKEN";
            artifactDef.smallIconSelectedSprite = GeneticsArtifactPlugin.geneticAssetBundle.LoadAsset<Sprite>("Assets/Genetics/Selected.png");
            artifactDef.smallIconDeselectedSprite = GeneticsArtifactPlugin.geneticAssetBundle.LoadAsset<Sprite>("Assets/Genetics/Unselected.png");
            artifactDef.pickupModelPrefab = GeneticsArtifactPlugin.geneticAssetBundle.LoadAsset<GameObject>("Assets/Genetics/PickupGene.prefab");
            ArtifactAPI.Add(artifactDef);

            geneArtifactCompoundDef = ScriptableObject.CreateInstance<ArtifactCompoundDef>();
            geneArtifactCompoundDef.modelPrefab = GeneticsArtifactPlugin.geneticAssetBundle.LoadAsset<GameObject>("Assets/Genetics/CompoundGene.prefab");
            geneArtifactCompoundDef.value = 15;
            ArtifactCodeAPI.AddCompound(geneArtifactCompoundDef);

            artifactCode = ScriptableObject.CreateInstance<ArtifactCode>();
            artifactCode.ArtifactCompounds = new List<int> { ArtifactCodeAPI.CompoundValues.Triangle, ArtifactCodeAPI.CompoundValues.Diamond, ArtifactCodeAPI.CompoundValues.Triangle,
                                                             ArtifactCodeAPI.CompoundValues.Empty,    geneArtifactCompoundDef.value,          ArtifactCodeAPI.CompoundValues.Empty,
                                                             ArtifactCodeAPI.CompoundValues.Triangle, ArtifactCodeAPI.CompoundValues.Diamond, ArtifactCodeAPI.CompoundValues.Triangle};
            ArtifactCodeAPI.AddCode(artifactDef, artifactCode);
        }
    }
}