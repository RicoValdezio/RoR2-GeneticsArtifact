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

            LanguageAPI.Add("GENE_NAME_TOKEN", "Artifact of Genetics");
            LanguageAPI.Add("GENE_DESC_TOKEN", "Monsters' stats will change based on the performance of previous monsters.");

            def.nameToken = "GENE_NAME_TOKEN";
            def.descriptionToken = "GENE_DESC_TOKEN";
            def.smallIconSelectedSprite = Resources.Load<Sprite>("@Genetics:Assets/Genetics/Selected.png");
            def.smallIconDeselectedSprite = Resources.Load<Sprite>("@Genetics:Assets/Genetics/Unselected.png");

            ArtifactCatalog.getAdditionalEntries += ArtifactCatalog_getAdditionalEntries;
        }

        private static void ArtifactCatalog_getAdditionalEntries(System.Collections.Generic.List<ArtifactDef> obj)
        {
            obj.Add(def);
        }
    }
}