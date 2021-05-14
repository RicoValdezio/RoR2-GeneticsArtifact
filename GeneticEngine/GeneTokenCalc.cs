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
            geneValue += 0.01f * body.inventory?.GetItemCount(GeneTokens.tokenDict[statType][GeneMod.Plus1]) ?? 0f;
            geneValue -= 0.01f * body.inventory?.GetItemCount(GeneTokens.tokenDict[statType][GeneMod.Minus1]) ?? 0f;
            return geneValue;
        }

        /// <summary>
        /// Builds a ItemInfo[] that holds the items needed to reflect a gene change
        /// </summary>
        /// <param name="oldValues">The current multipliers that are to be replaced</param>
        /// <param name="newValues">The replacement multipliers </param>
        /// <returns>An ItemInfo[] that contains only GeneTokens that can be added/copied to another Inventory</returns>
        public static Dictionary<ItemDef, int> GetTokensToAdd(Dictionary<GeneStat, float> oldValues, Dictionary<GeneStat, float> newValues)
        {
            Dictionary<ItemDef, int> tokensToGive = new Dictionary<ItemDef, int>();
            Dictionary<GeneStat, float> diffValues = new Dictionary<GeneStat, float>();
            foreach (GeneStat stat in Enum.GetValues(typeof(GeneStat)))
            {
                diffValues.Add(stat, newValues[stat] - oldValues[stat]);
            }

            if (diffValues[GeneStat.MaxHealth] > 0)
            {
                tokensToGive.Add(GeneTokens.tokenDict[GeneStat.MaxHealth][GeneMod.Plus1], (int)(diffValues[GeneStat.MaxHealth] * 100));
            }
            else
            {
                tokensToGive.Add(GeneTokens.tokenDict[GeneStat.MaxHealth][GeneMod.Minus1], (int)(diffValues[GeneStat.MaxHealth] * -100));
            }

            if (diffValues[GeneStat.MoveSpeed] > 0)
            {
                tokensToGive.Add(GeneTokens.tokenDict[GeneStat.MoveSpeed][GeneMod.Plus1], (int)(diffValues[GeneStat.MoveSpeed] * 100));
            }
            else
            {
                tokensToGive.Add(GeneTokens.tokenDict[GeneStat.MoveSpeed][GeneMod.Minus1], (int)(diffValues[GeneStat.MoveSpeed] * -100));
            }

            if (diffValues[GeneStat.AttackSpeed] > 0)
            {
                tokensToGive.Add(GeneTokens.tokenDict[GeneStat.AttackSpeed][GeneMod.Plus1], (int)(diffValues[GeneStat.AttackSpeed] * 100));
            }
            else
            {
                tokensToGive.Add(GeneTokens.tokenDict[GeneStat.AttackSpeed][GeneMod.Minus1], (int)(diffValues[GeneStat.AttackSpeed] * -100));
            }

            if (diffValues[GeneStat.AttackDamage] > 0)
            {
                tokensToGive.Add(GeneTokens.tokenDict[GeneStat.AttackDamage][GeneMod.Plus1], (int)(diffValues[GeneStat.AttackDamage] * 100));
            }
            else
            {
                tokensToGive.Add(GeneTokens.tokenDict[GeneStat.AttackDamage][GeneMod.Minus1], (int)(diffValues[GeneStat.AttackDamage] * -100));
            }

            return tokensToGive;
        }
        #endregion
    }
}
