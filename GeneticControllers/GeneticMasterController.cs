using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace GeneticsArtifact
{
    internal class GeneticMasterController : MonoBehaviour
    {
        internal static List<GeneTracker> masterTrackers;
        internal static List<GeneTracker> deadTrackers;
        internal static List<GeneBehaviour> livingBehaviours;

        //Configure the timeBetweenUpdates
        internal static float updateTimer = 0f, statusTimer = 0f;

        internal static bool rapidMutationActive, moonActive;
        internal static float rapidTimer = 0f;

        internal static void Init()
        {
            masterTrackers = new List<GeneTracker>();
            deadTrackers = new List<GeneTracker>();
            livingBehaviours = new List<GeneBehaviour>();
            rapidMutationActive = ConfigMaster.rapidMutationType.Contains("Always");

            On.RoR2.CharacterBody.Start += CharacterBody_Start;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.Run.BeginGameOver += Run_BeginGameOver;
            On.RoR2.RunArtifactManager.SetArtifactEnabledServer += RunArtifactManager_SetArtifactEnabledServer;

            On.RoR2.Stage.Start += Stage_Start;
            On.RoR2.HoldoutZoneController.OnEnable += HoldoutZoneController_OnEnable;
            On.RoR2.HoldoutZoneController.OnDisable += HoldoutZoneController_OnDisable;
        }

        private void Update()
        {
            //If the artifact is enabled
            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def.artifactIndex))
            {
                #region Logging
                updateTimer += Time.deltaTime;
                if (updateTimer >= ConfigMaster.timeBetweenUpdates)
                {
                    //If the specified time has passed, update the masters and purge the dead
                    updateTimer = 0f;
                    //Chat.AddMessage("Dead Masters Count : " + deadTrackers.Count.ToString());
                    foreach (GeneTracker masterTracker in masterTrackers)
                    {
                        masterTracker.MutateFromChildren();
                    }
                    deadTrackers.Clear();
                }

                //Status logging for those who have it enabled
                if (ConfigMaster.statusLogging) {
                    statusTimer += Time.deltaTime;
                    if (statusTimer >= ConfigMaster.timeBetweenStatusLogging)
                    {
                        statusTimer = 0f;
                        GeneticsArtifactPlugin.geneticLogSource.LogInfo("Begin Genetic Master Status Log");
                        foreach (GeneTracker masterTracker in masterTrackers)
                        {
                            GeneticsArtifactPlugin.geneticLogSource.LogInfo(masterTracker.GetGeneString());
                        }
                        GeneticsArtifactPlugin.geneticLogSource.LogInfo("End Genetic Master Status Log");
                    } 
                }
                #endregion

                
                //else if (ConfigMaster.rapidMutationType.Contains("Event"))
                //{
                //    rapidBroadcast = InstanceTracker.GetInstancesList<HoldoutZoneController>().Count > 0;
                //}
                //else
                //{
                //    rapidBroadcast = false;
                //}

                #region RapidMutation-Running
                if (rapidMutationActive)
                {
                    rapidTimer += Time.deltaTime;
                    if(rapidTimer >= 1f)
                    {
                        rapidTimer = 0f;
                        foreach(GeneBehaviour behaviour in livingBehaviours)
                        {
                            behaviour.tracker.MutateSelf();
                            behaviour.ApplyMutation();
                        }
                    }
                }
                else
                {
                    rapidTimer = 0f;
                }
                #endregion
            }
        }

        #region GeneralHooks
        private static void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            //If the artifact is enabled and the body is a monster
            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def.artifactIndex))
            {
                //Always apply this to Monsters, optionally apply this to Player minions and Neutrals
                if ((self.teamComponent.teamIndex == TeamIndex.Monster) ||
                    (self.teamComponent.teamIndex == TeamIndex.Neutral && ConfigMaster.applyToNeutrals) ||
                    (self.teamComponent.teamIndex == TeamIndex.Player && ConfigMaster.applyToMinions && !self.master.playerCharacterMasterController))
                {
                    //If using a master per monster type and there isn't already a master for this type, add a master for this type
                    if (ConfigMaster.trackerPerMonsterID && masterTrackers.Find(x => x.index == self.bodyIndex) == null)
                    {
                        masterTrackers.Add(new GeneTracker(self.bodyIndex, true));
                        //Chat.AddMessage("A new Master was made for bodyIndex: " + body.baseNameToken);
                    }
                    else if(masterTrackers.Count < ConfigMaster.maxTrackers)
                    {
                        for (int x = masterTrackers.Count; x < ConfigMaster.maxTrackers; x++)
                        {
                            masterTrackers.Add(new GeneTracker(x, true));
                        }
                    }
                    //Always add a behaviour to the body
                    self.gameObject.AddComponent<GeneBehaviour>();
                }
            }
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def.artifactIndex))
            {
                if (!float.IsNaN(damageInfo.damage) && damageInfo.damage > 0)
                {
                    foreach (GeneBehaviour behaviour in livingBehaviours)
                    {
                        //If behaviour body matches, add its damage and break out
                        if (damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>() == behaviour.body)
                        {
                            behaviour.damageDealt += damageInfo.damage;
                            break;
                        }
                        if (damageInfo.inflictor && damageInfo.inflictor.GetComponent<CharacterBody>() == behaviour.body)
                        {
                            behaviour.damageDealt += damageInfo.damage;
                            break;
                        }
                    }
                }
            }
        }

        private static void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            bool found;

            #region HealthMultiplier
            found = c.TryGotoNext(
                        x => x.MatchLdfld<CharacterBody>("baseMaxHealth"),
                        x => x.MatchLdarg(0),
                        x => x.MatchLdfld<CharacterBody>("levelMaxHealth"))
                    && c.TryGotoNext(
                        x => x.MatchAdd());
            if (found)
            {
                c.GotoNext(x => x.MatchStloc(out _));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origHealth, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origHealth * geneBehaviour.tracker.GetGeneValue("Health");
                    }
                    else
                    {
                        return origHealth;
                    }
                });
            }
            else
            {
                GeneticsArtifactPlugin.geneticLogSource.LogError("Health Hook Failed to Register");
            }
            c.Index = 0;
            #endregion

            #region RegenMultiplier
            //For some reason num39 (The base+level regen) is held in memory (isn't stored), so we have to intercept it when its alone.
            found = c.TryGotoNext(
                        x => x.MatchLdfld<CharacterBody>("baseRegen"),
                        x => x.MatchLdarg(0),
                        x => x.MatchLdfld<CharacterBody>("levelRegen"))
                    && c.TryGotoNext(
                        x => x.MatchAdd());
            if (found)
            {
                c.GotoNext(x => x.MatchStloc(out _));
                c.GotoNext(x => x.MatchLdloc(out _));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origRegen, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origRegen * geneBehaviour.tracker.GetGeneValue("Regen");
                    }
                    else
                    {
                        return origRegen;
                    }
                });
            }
            else
            {
                GeneticsArtifactPlugin.geneticLogSource.LogError("Health Hook Failed to Register");
            }
            c.Index = 0;
            #endregion

            #region MoveSpeedMultiplier
            found = c.TryGotoNext(
                    x => x.MatchLdfld<CharacterBody>("baseMoveSpeed"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelMoveSpeed"))
                && c.TryGotoNext(
                    x => x.MatchAdd());
            if (found)
            {
                c.GotoNext(x => x.MatchStloc(out _));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origMoveSpeed, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origMoveSpeed * geneBehaviour.tracker.GetGeneValue("MoveSpeed");
                    }
                    else
                    {
                        return origMoveSpeed;
                    }
                });
            }
            else
            {
                GeneticsArtifactPlugin.geneticLogSource.LogError("MoveSpeed Hook Failed to Register");
            }
            c.Index = 0;
            #endregion

            #region DamageMultiplier
            found = c.TryGotoNext(
                    x => x.MatchLdfld<CharacterBody>("baseDamage"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelDamage"))
                && c.TryGotoNext(
                    x => x.MatchAdd());
            if (found)
            {
                c.GotoNext(x => x.MatchStloc(out _));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origDamage, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origDamage * geneBehaviour.tracker.GetGeneValue("Damage");
                    }
                    else
                    {
                        return origDamage;
                    }
                });
            }
            else
            {
                GeneticsArtifactPlugin.geneticLogSource.LogError("Damage Hook Failed to Register");
            }
            c.Index = 0;
            #endregion

            #region AttackSpeedMultiplier
            found = c.TryGotoNext(
                    x => x.MatchLdfld<CharacterBody>("baseAttackSpeed"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelAttackSpeed"))
                && c.TryGotoNext(
                    x => x.MatchAdd());
            if (found)
            {
                c.GotoNext(x => x.MatchStloc(out _));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origAttackSpeed, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origAttackSpeed * geneBehaviour.tracker.GetGeneValue("AttackSpeed");
                    }
                    else
                    {
                        return origAttackSpeed;
                    }
                });
            }
            else
            {
                GeneticsArtifactPlugin.geneticLogSource.LogError("AttackSpeed Hook Failed to Register");
            }
            c.Index = 0;
            #endregion

            #region ArmorMultiplier
            //Armor is also never stored, so we have to intercept it in the single line.
            found = c.TryGotoNext(
                    x => x.MatchLdfld<CharacterBody>("baseArmor"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelArmor"))
                && c.TryGotoNext(
                    x => x.MatchAdd());
            if (found)
            {
                c.GotoNext();
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origArmor, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origArmor * geneBehaviour.tracker.GetGeneValue("Armor");
                    }
                    else
                    {
                        return origArmor;
                    }
                });
            }
            else
            {
                GeneticsArtifactPlugin.geneticLogSource.LogError("Armor Hook Failed to Register");
            }
            c.Index = 0;
            #endregion
        }

        private static void PurgeMasters()
        {
            masterTrackers.Clear();
            deadTrackers.Clear();
        }

        private static void Run_BeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameEndingDef gameEndingDef)
        {
            orig(self, gameEndingDef);
            PurgeMasters();
        }

        private static void RunArtifactManager_SetArtifactEnabledServer(On.RoR2.RunArtifactManager.orig_SetArtifactEnabledServer orig, RunArtifactManager self, ArtifactDef artifactDef, bool newEnabled)
        {
            orig(self, artifactDef, newEnabled);
            //If the artifact is being disbled
            if (artifactDef.artifactIndex == ArtifactOfGenetics.def.artifactIndex && !newEnabled)
            {
                //If pausing is disabled, purge the masters and the dead
                if (!ConfigMaster.enableMasterPause)
                {
                    GeneticsArtifactPlugin.geneticLogSource.LogInfo("Artifact has been disabled, purging all masters.");
                    PurgeMasters();
                }
                else
                {
                    GeneticsArtifactPlugin.geneticLogSource.LogInfo("Artifact has been disabled, pausing all masters.");
                }
            }
        }
        #endregion

        #region RapidMutation-EventTriggers
        public static void UpdateRapidMutation(bool newValue)
        {
            if (newValue != rapidMutationActive)
            {
                GeneticsArtifactPlugin.geneticLogSource.LogInfo("Rapid Mutation has been " + (newValue ? "Activated" : "Deactivated"));
                Chat.AddMessage("The Aritfact of Genetics " + (newValue ? "has started glowing!" : "glow has subsided."));
                rapidMutationActive = newValue;
            }
        }

        private static void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            if (self.sceneDef.isFinalStage && ConfigMaster.rapidMutationType.Contains("Moon"))
            {
                UpdateRapidMutation(true);
                moonActive = true;
            }
            else
            {
                UpdateRapidMutation(false);
                moonActive = false;
            }
        }

        private static void HoldoutZoneController_OnEnable(On.RoR2.HoldoutZoneController.orig_OnEnable orig, HoldoutZoneController self)
        {
            orig(self);
            if (ConfigMaster.rapidMutationType.Contains("Event"))
            {
                UpdateRapidMutation(true);
            }
        }

        private static void HoldoutZoneController_OnDisable(On.RoR2.HoldoutZoneController.orig_OnDisable orig, HoldoutZoneController self)
        {
            orig(self);
            if (ConfigMaster.rapidMutationType.Contains("Event") && !moonActive && InstanceTracker.GetInstancesList<HoldoutZoneController>().Count == 0)
            {
                UpdateRapidMutation(false);
            }
        }
        #endregion
    }
}