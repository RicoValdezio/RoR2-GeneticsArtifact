using R2API;
using R2API.ScriptableObjects;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticsArtifact
{
    public class ArtifactOfGenetics
    {
        public static ArtifactDef def;
        public static ArtifactCode code;

        internal static void Init()
        {
            def = ScriptableObject.CreateInstance<ArtifactDef>();

            LanguageAPI.Add("GENETIC_ARTIFACT_NAME_TOKEN", "Artifact of Genetics");
            LanguageAPI.Add("GENETIC_ARTIFACT_DESCRIPTION_TOKEN", "Monsters will spawn with adjusted stats. Adjustments are determined by a genetic algorithm.");

            def.nameToken = "GENETIC_ARTIFACT_NAME_TOKEN";
            def.descriptionToken = "GENETIC_ARTIFACT_DESCRIPTION_TOKEN";
            def.smallIconSelectedSprite = GeneticsArtifactPlugin.geneticAssetBundle.LoadAsset<Sprite>("Assets/Genetics/Selected.png");
            def.smallIconDeselectedSprite = GeneticsArtifactPlugin.geneticAssetBundle.LoadAsset<Sprite>("Assets/Genetics/Unselected.png");
            def.pickupModelPrefab = GeneticsArtifactPlugin.geneticAssetBundle.LoadAsset<GameObject>("Assets/Genetics/PickupGene.prefab");

            ArtifactAPI.Add(def);

            code = ScriptableObject.CreateInstance<ArtifactCode>();
            code.ArtifactCompounds = new List<int> { ArtifactCodeAPI.CompoundValues.Triangle, ArtifactCodeAPI.CompoundValues.Diamond, ArtifactCodeAPI.CompoundValues.Triangle,
                                                     ArtifactCodeAPI.CompoundValues.Empty,    ArtifactCodeAPI.CompoundValues.Square,  ArtifactCodeAPI.CompoundValues.Empty,
                                                     ArtifactCodeAPI.CompoundValues.Triangle, ArtifactCodeAPI.CompoundValues.Diamond, ArtifactCodeAPI.CompoundValues.Triangle};

            ArtifactCodeAPI.Add(def, code);
        }
    }
}