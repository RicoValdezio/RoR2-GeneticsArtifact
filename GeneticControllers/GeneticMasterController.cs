using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace GeneticsArtifact
{
    internal class GeneticMasterController
    {
        internal static List<GeneTracker> masterTrackers;
        internal static List<GeneTracker> deadTrackers;
        internal static List<GeneBehaviour> livingBehaviours;

        //Configure the timeBetweenUpdates
        internal static float updateTimer = 0f, statusTimer = 0f;

        internal static void Init()
        {
            masterTrackers = new List<GeneTracker>();
            deadTrackers = new List<GeneTracker>();
            livingBehaviours = new List<GeneBehaviour>();

            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.CharacterBody.Start += CharacterBody_Start;
            On.RoR2.Run.Update += Run_Update;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private static void BuildMasters()
        {
            if (ConfigMaster.trackerPerMonsterID)
            {
                //Do nothing, we'll add trackers when the first of a monster spawns
            }
            else
            {
                for (int x = 0; x < ConfigMaster.maxTrackers; x++)
                {
                    masterTrackers.Add(new GeneTracker(x, true));
                }
            }
        }

        private static void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);
            //If the artifact is enabled
            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def.artifactIndex))
            {
                //If its the first stage in a run, reset all masters and purge all dead
                if (self.stageClearCount == 0)
                {
                    masterTrackers.Clear();
                    deadTrackers.Clear();
                    BuildMasters();
                }
            }
        }

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
                    //Always add a behaviour to the body
                    self.gameObject.AddComponent<GeneBehaviour>();
                }
            }
        }

        private static void Run_Update(On.RoR2.Run.orig_Update orig, Run self)
        {
            orig(self);
            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def.artifactIndex))
            {
                updateTimer += Time.deltaTime;
                statusTimer += Time.deltaTime;
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
                if (ConfigMaster.statusLogging && statusTimer >= ConfigMaster.timeBetweenStatusLogging)
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
    }
}