using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace GeneticsArtifact
{
    public class GeneEngineDriver : NetworkBehaviour
    {
        public static GeneEngineDriver instance;
        public static List<MasterGeneBehaviour> masterGenes;
        public static List<MonsterGeneBehaviour> livingGenes, deadGenes;

        #region Hooks
        public static void RegisterHooks()
        {
            On.RoR2.Run.Start += Run_Start;
            On.RoR2.CharacterBody.Start += CharacterBody_Start;
        }

        private static void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            self.gameObject.AddComponent<GeneEngineDriver>();
        }

        private static void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            if (!NetworkServer.active) return;

            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def) &&
                self.teamComponent.teamIndex == TeamIndex.Monster)
            {
                if (!masterGenes.Exists(x => x.bodyIndex == self.bodyIndex))
                {
                    MasterGeneBehaviour newMaster = new MasterGeneBehaviour();
                    newMaster.Init();
                    newMaster.bodyIndex = self.bodyIndex;
                    masterGenes.Add(newMaster);
                    GeneticsArtifactPlugin.geneticLogSource.LogInfo("Generated a Master Template for: " + BodyCatalog.GetBodyName(self.bodyIndex));
                }

                MonsterGeneBehaviour geneBehaviour = self.gameObject.AddComponent<MonsterGeneBehaviour>();
                geneBehaviour.MutateFromMaster();
            }
        }
        #endregion

        public void Awake()
        {
            if (instance == null) instance = this;
            if (!NetworkServer.active)
            {
                //Only the host should be running this object, so destroy it for non-hosts
                Destroy(this);
                return;
            }

            masterGenes = new List<MasterGeneBehaviour>();
            livingGenes = new List<MonsterGeneBehaviour>();
            deadGenes = new List<MonsterGeneBehaviour>();
        }
    }
}
