using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GeneticsArtifact
{
    public class GeneEngineDriver : NetworkBehaviour
    {
        public static GeneEngineDriver instance;
        public static List<MasterGeneBehaviour> masterGenes;
        public static List<MonsterGeneBehaviour> livingGenes, deadGenes;
        public static float timeSinceLastLearning = 0f;

        #region Hooks
        public static void RegisterHooks()
        {
            On.RoR2.Run.Start += Run_Start;
            On.RoR2.CharacterBody.Start += CharacterBody_Start;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private static void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            self.gameObject.AddComponent<GeneEngineDriver>();
        }

        private static void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def) &&
                    self.teamComponent.teamIndex == TeamIndex.Monster &&
                    self.inventory)
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
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            if (NetworkServer.active)
            {
                if (damageInfo.attacker?.GetComponent<CharacterBody>() is CharacterBody attackerBody)
                {
                    MonsterGeneBehaviour attackerGene = livingGenes.Find(x => x.characterBody == attackerBody);
                    if (attackerGene) attackerGene.damageDealt += damageInfo.damage;
                }
                else if (damageInfo.inflictor?.GetComponent<CharacterBody>() is CharacterBody inflictorBody)
                {
                    MonsterGeneBehaviour inflictorGene = livingGenes.Find(x => x.characterBody == inflictorBody);
                    if (inflictorGene) inflictorGene.damageDealt += damageInfo.damage;
                }
            }
        }
        #endregion

        public void Awake()
        {
            if (instance == null) instance = this;
            if (!NetworkServer.active) return;

            masterGenes = new List<MasterGeneBehaviour>();
            livingGenes = new List<MonsterGeneBehaviour>();
            deadGenes = new List<MonsterGeneBehaviour>();
        }

        public void Update()
        {
            if (!NetworkServer.active) return;
            timeSinceLastLearning += Time.deltaTime;
            //Every 20 seconds or every 10 deaths
            if (timeSinceLastLearning >= 60f || deadGenes.Count >= 40)
            {
                foreach (MasterGeneBehaviour master in masterGenes)
                {
                    master.MutateFromChildren();
                }
                deadGenes.Clear();
            }
        }
    }
}
