﻿using RoR2;
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
            if (NetworkServer.active && RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.artifactDef))
            {
                self.gameObject.AddComponent<GeneEngineDriver>();
            }
        }

        private static void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active && 
               (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.artifactDef) || ConfigManager.maintainIfDisabled.Value))
            {
                if (instance == null) //Emergency Catch for Bulwark Edge Case
                {
                    Run.instance.gameObject.AddComponent<GeneEngineDriver>();
                    GeneticsArtifactPlugin.geneticLogSource.LogWarning("GeneEngineDriver Emergency Activation! Wasn't ready for a body yet.");
                }
                if (self.teamComponent.teamIndex == TeamIndex.Monster &&
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
                    if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.artifactDef)) geneBehaviour.MutateFromMaster();
                    else if (ConfigManager.maintainIfDisabled.Value) geneBehaviour.CopyFromMaster();
                }
            }
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            if (NetworkServer.active && RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.artifactDef))
            {
                if (damageInfo.attacker is GameObject attackerObject)
                {
                    if (attackerObject != null && attackerObject.GetComponent<CharacterBody>() is CharacterBody attackerBody)
                    {
                        if (attackerBody != null && attackerBody.inventory?.GetItemCount(GeneTokens.blockerDef) == 0)
                        {
                            MonsterGeneBehaviour attackerGene = livingGenes.Find(x => x.characterBody == attackerBody);
                            if (attackerGene != null) attackerGene.damageDealt += damageInfo.damage;
                        }
                    }
                }
                else if (damageInfo.inflictor is GameObject inflictorObject)
                {
                    if (inflictorObject != null && inflictorObject.GetComponent<CharacterBody>() is CharacterBody inflictorBody)
                    {
                        if (inflictorBody != null && inflictorBody.inventory?.GetItemCount(GeneTokens.blockerDef) == 0)
                        {
                            MonsterGeneBehaviour inflictorGene = livingGenes.Find(x => x.characterBody == inflictorBody);
                            if (inflictorGene != null) inflictorGene.damageDealt += damageInfo.damage;
                        }
                    }
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
            switch (ConfigManager.governorType.Value)
            {
                case (int)GovernorType.TimeOnly:
                    if (timeSinceLastLearning >= (float)ConfigManager.timeLimit.Value) Learn();
                    break;
                case (int)GovernorType.DeathsOnly:
                    if (deadGenes.Count >= ConfigManager.deathLimit.Value) Learn();
                    break;
                default:
                    if (timeSinceLastLearning >= (float)ConfigManager.timeLimit.Value || deadGenes.Count >= ConfigManager.deathLimit.Value) Learn();
                    break;
            }
        }

        public void Learn()
        {
#if DEBUG
            GeneticsArtifactPlugin.geneticLogSource.LogInfo("Running Learn with Timer: " + timeSinceLastLearning + " and Deaths: " + deadGenes.Count);
#endif
            foreach (MasterGeneBehaviour master in masterGenes)
            {
                master.MutateFromChildren();
            }
            deadGenes.Clear();
            timeSinceLastLearning = 0f;
        }
    }
}
