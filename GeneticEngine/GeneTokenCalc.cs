using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;

namespace GeneticsArtifact
{
    public class GeneTokenCalc
    {
        #region Hooks
        public static void RegisterHooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats_GeneTokens;
        }

        private static void CharacterBody_RecalculateStats_GeneTokens(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            bool found = false;

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
                    return origHealth * GetGeneMultiplier(body, GeneStat.MaxHealth);
                });
            }
            else
            {
                GeneticsArtifactPlugin.geneticLogSource.LogError("Health Hook Failed to Register");
            }
            c.Index = 0;
            found = false;

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
                    return origMoveSpeed * GetGeneMultiplier(body, GeneStat.MoveSpeed);
                });
            }
            else
            {
                GeneticsArtifactPlugin.geneticLogSource.LogError("MoveSpeed Hook Failed to Register");
            }
            c.Index = 0;
            found = false;

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
                    return origDamage * GetGeneMultiplier(body, GeneStat.AttackDamage);
                });
            }
            else
            {
                GeneticsArtifactPlugin.geneticLogSource.LogError("Damage Hook Failed to Register");
            }
            c.Index = 0;
            found = false;

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
                    return origAttackSpeed * GetGeneMultiplier(body, GeneStat.AttackSpeed);
                });
            }
            else
            {
                GeneticsArtifactPlugin.geneticLogSource.LogError("AttackSpeed Hook Failed to Register");
            }
            c.Index = 0;
            found = false;
        }
        #endregion

        #region Calculators
        /// <summary>
        /// Used to determine the current gene multiplier for a given CharacterBody
        /// </summary>
        /// <param name="body">The body in question, needs an inventory</param>
        /// <param name="statType">The stat being checked</param>
        /// <returns>The stat multiplier based on the current items</returns>
        public static float GetGeneMultiplier(CharacterBody body, GeneStat statType)
        {
            float geneValue = 1;
            geneValue += 1.00f * body.inventory?.GetItemCount(GeneTokens.tokenDict[statType][GeneMod.Plus100]) ?? 0f;
            geneValue += 0.25f * body.inventory?.GetItemCount(GeneTokens.tokenDict[statType][GeneMod.Plus25]) ?? 0f;
            geneValue += 0.05f * body.inventory?.GetItemCount(GeneTokens.tokenDict[statType][GeneMod.Plus5]) ?? 0f;
            geneValue += 0.01f * body.inventory?.GetItemCount(GeneTokens.tokenDict[statType][GeneMod.Plus1]) ?? 0f;
            geneValue -= 0.01f * body.inventory?.GetItemCount(GeneTokens.tokenDict[statType][GeneMod.Minus1]) ?? 0f;
            geneValue -= 0.05f * body.inventory?.GetItemCount(GeneTokens.tokenDict[statType][GeneMod.Minus5]) ?? 0f;
            geneValue -= 0.25f * body.inventory?.GetItemCount(GeneTokens.tokenDict[statType][GeneMod.Minus25]) ?? 0f;
            return geneValue;
        }

        /// <summary>
        /// Builds a temporary Inventory that holds the items needed to reflect a gene change
        /// </summary>
        /// <param name="oldValues">The current multipliers that are to be replaced</param>
        /// <param name="newValues">The replacement multipliers </param>
        /// <returns>An Inventory that contains only GeneTokens that can be added/copied to another Inventory</returns>
        public static Inventory GetTokensToAdd(Dictionary<GeneStat, float> oldValues, Dictionary<GeneStat, float> newValues)
        {
            Inventory tokensToAdd = new Inventory();
            foreach (GeneStat stat in Enum.GetValues(typeof(GeneStat)))
            {
                float diff = newValues[stat] - oldValues[stat];
                while (diff != 0f)
                {
                    //This is a greedy approach to the change problem, it's not optimal but it's fast enough
                    if (diff > 0)
                    {
                        if (diff > 1.00f)
                        {
                            tokensToAdd.GiveItem(GeneTokens.tokenDict[stat][GeneMod.Plus100]);
                            diff -= 1.00f;
                            continue;
                        }
                        else if (diff > 0.25f)
                        {
                            tokensToAdd.GiveItem(GeneTokens.tokenDict[stat][GeneMod.Plus25]);
                            diff -= 0.25f;
                            continue;
                        }
                        else if (diff > 0.05f)
                        {
                            tokensToAdd.GiveItem(GeneTokens.tokenDict[stat][GeneMod.Plus5]);
                            diff -= 0.05f;
                            continue;
                        }
                        else
                        {
                            tokensToAdd.GiveItem(GeneTokens.tokenDict[stat][GeneMod.Plus1]);
                            diff -= 0.01f;
                            continue;
                        }
                    }
                    else
                    {
                        if (diff < -0.25f)
                        {
                            tokensToAdd.GiveItem(GeneTokens.tokenDict[stat][GeneMod.Minus25]);
                            diff += 0.25f;
                            continue;
                        }
                        else if (diff < -0.05f)
                        {
                            tokensToAdd.GiveItem(GeneTokens.tokenDict[stat][GeneMod.Minus5]);
                            diff += 0.05f;
                            continue;
                        }
                        else
                        {
                            tokensToAdd.GiveItem(GeneTokens.tokenDict[stat][GeneMod.Minus1]);
                            diff += 0.01f;
                            continue;
                        }
                    }
                }
            }
            return tokensToAdd;
        }
        #endregion
    }
}
