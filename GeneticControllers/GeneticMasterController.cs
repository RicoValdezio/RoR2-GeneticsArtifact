using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
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
        internal static int maxTrackers;
        internal static bool trackerPerMonsterID, applyToNeutrals, applyToMinions, statusLogging;

        //Configure the timeBetweenUpdates
        internal static float timeBetweenUpdates, updateTimer = 0f, timeBetweenStatusLogging, statusTimer = 0f;

        internal static void Init()
        {
            masterTrackers = new List<GeneTracker>();
            deadTrackers = new List<GeneTracker>();
            livingBehaviours = new List<GeneBehaviour>();

            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.CharacterMaster.SpawnBody += CharacterMaster_SpawnBody;
            On.RoR2.Run.Update += Run_Update;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private static void BuildMasters()
        {
            if (trackerPerMonsterID)
            {
                //Do nothing, we'll add trackers when the first of a monster spawns
            }
            else
            {
                for (int x = 0; x < maxTrackers; x++)
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

        private static CharacterBody CharacterMaster_SpawnBody(On.RoR2.CharacterMaster.orig_SpawnBody orig, CharacterMaster self, GameObject bodyPrefab, Vector3 position, Quaternion rotation)
        {
            CharacterBody body = orig(self, bodyPrefab, position, rotation);
            //If the artifact is enabled and the body is a monster
            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def.artifactIndex))
            {
                //Always apply this to Monsters, optionally apply this to Player minions and Neutrals
                if ((body.teamComponent.teamIndex == TeamIndex.Monster) ||
                    (body.teamComponent.teamIndex == TeamIndex.Neutral && applyToNeutrals) ||
                    (body.teamComponent.teamIndex == TeamIndex.Player && applyToMinions && !body.master.playerCharacterMasterController))
                {
                    //If using a master per monster type and there isn't already a master for this type, add a master for this type
                    if (trackerPerMonsterID && masterTrackers.Find(x => x.index == body.bodyIndex) == null)
                    {
                        masterTrackers.Add(new GeneTracker(body.bodyIndex, true));
                        //Chat.AddMessage("A new Master was made for bodyIndex: " + body.baseNameToken);
                    }
                    //Always add a behaviour to the body
                    body.gameObject.AddComponent<GeneBehaviour>();
                }
            }
            return body;
        }

        private static void Run_Update(On.RoR2.Run.orig_Update orig, Run self)
        {
            orig(self);
            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def.artifactIndex))
            {
                updateTimer += Time.deltaTime;
                statusTimer += Time.deltaTime;
                if (updateTimer >= timeBetweenUpdates)
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
                if (statusLogging && statusTimer >= timeBetweenStatusLogging)
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
            //A lot of this is based on ThinkInvis's TLIER, adjusted to fit my style and needs
            ILCursor c = new ILCursor(il);
            bool found;

            #region HealthMultiplier
            int healthIndex = -1;
            found = c.TryGotoNext(
                    x => x.MatchLdfld<CharacterBody>("baseMaxHealth"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CharacterBody>("levelMaxHealth"))
                    && c.TryGotoNext(
                    x => x.MatchStloc(out healthIndex));
            if (found)
            {
                c.GotoPrev(x => x.MatchLdfld<CharacterBody>("baseMaxHealth"));
                c.GotoNext(x => x.MatchStloc(healthIndex));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origHealth, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origHealth * geneBehaviour.tracker.genes[0];
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
            int regenIndex = -1;
            found = c.TryGotoNext(
                x => x.MatchLdloc(out regenIndex),
                x => x.MatchCallOrCallvirt<CharacterBody>("set_regen")
                );
            if (found)
            {
                c.GotoPrev(
                    x => x.MatchAdd(),
                    x => x.MatchStloc(regenIndex)
                    );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origRegen, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origRegen * geneBehaviour.tracker.genes[1];
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
            int speedIndex = -1;
            found = c.TryGotoNext(
                x => x.MatchLdfld<CharacterBody>("baseMoveSpeed"),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<CharacterBody>("levelMoveSpeed"))
                && c.TryGotoNext(
                x => x.MatchStloc(out speedIndex)
                );
            if (found)
            {
                c.GotoPrev(x => x.MatchLdfld<CharacterBody>("levelMoveSpeed"));
                c.GotoNext(x => x.MatchStloc(speedIndex));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origMoveSpeed, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origMoveSpeed * geneBehaviour.tracker.genes[2];
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

            #region AccelMultiplier
            found = c.TryGotoNext(
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<CharacterBody>("baseAcceleration"),
                x => x.MatchMul()
                );
            if (found)
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origAccel, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origAccel * geneBehaviour.tracker.genes[3];
                    }
                    else
                    {
                        return origAccel;
                    }
                });
            }
            else
            {
                GeneticsArtifactPlugin.geneticLogSource.LogError("Acceleration Hook Failed to Register");
            }
            c.Index = 0;
            #endregion

            #region DamageMultiplier
            int damageIndex = -1;
            found = c.TryGotoNext(
                x => x.MatchLdfld<CharacterBody>("baseDamage"),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<CharacterBody>("levelDamage"))
                && c.TryGotoNext(
                    x => x.MatchStloc(out damageIndex)
                );
            if (found)
            {
                c.GotoPrev(x => x.MatchLdfld<CharacterBody>("baseDamage"));
                c.GotoNext(x => x.MatchStloc(damageIndex));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origDamage, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origDamage * geneBehaviour.tracker.genes[4];
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
            int attackSpeedIndex = -1;
            found = c.TryGotoNext(
                x => x.MatchLdfld<CharacterBody>("baseAttackSpeed"),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<CharacterBody>("levelAttackSpeed"))
                && c.TryGotoNext(
                    x => x.MatchStloc(out attackSpeedIndex)
                );
            if (found)
            {
                c.GotoPrev(x => x.MatchLdfld<CharacterBody>("baseAttackSpeed"));
                c.GotoNext(x => x.MatchStloc(attackSpeedIndex));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origAttackSpeed, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origAttackSpeed * geneBehaviour.tracker.genes[5];
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
            int armorIndex = -1;
            found = c.TryGotoNext(
                x => x.MatchLdfld<CharacterBody>("baseArmor"),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<CharacterBody>("levelArmor"))
                && c.TryGotoNext(
                    x => x.MatchStloc(out armorIndex)
                );
            if (found)
            {
                c.GotoPrev(x => x.MatchLdfld<CharacterBody>("baseArmor"));
                c.GotoNext(x => x.MatchStloc(armorIndex));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((origArmor, body) =>
                {
                    if (body?.gameObject?.GetComponent<GeneBehaviour>() is GeneBehaviour geneBehaviour)
                    {
                        return origArmor * geneBehaviour.tracker.genes[6];
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