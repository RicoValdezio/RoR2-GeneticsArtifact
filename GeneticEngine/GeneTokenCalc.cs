using R2API;
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
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory.GetItemCount(GeneTokens.blockerDef) == 0)
            {
                args.baseHealthAdd += GetStatValueToAdd(sender, GeneStat.MaxHealth);
                args.baseMoveSpeedAdd += GetStatValueToAdd(sender, GeneStat.MoveSpeed);
                args.baseAttackSpeedAdd += GetStatValueToAdd(sender, GeneStat.AttackSpeed);
                args.baseDamageAdd += GetStatValueToAdd(sender, GeneStat.AttackDamage);
            }
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
        /// Used to determine the float value that needs to be added to the baseStatAdd value
        /// </summary>
        /// <param name="body">The body in question, will be passed to GetGeneMultiplier</param>
        /// <param name="statType">The stat being checked</param>
        /// <returns>The stat modifier, this is to be added to the base stat</returns>
        public static float GetStatValueToAdd(CharacterBody body, GeneStat statType)
        {
            float baseStatValue = 0f;
            switch (statType)
            {
                case GeneStat.MaxHealth:
                    baseStatValue = body.baseMaxHealth + (body.levelMaxHealth * body.level);
                    break;
                case GeneStat.MoveSpeed:
                    baseStatValue = body.baseMoveSpeed + (body.levelMoveSpeed * body.level);
                    break;
                case GeneStat.AttackSpeed:
                    baseStatValue = body.baseAttackSpeed + (body.levelAttackSpeed * body.level);
                    break;
                case GeneStat.AttackDamage:
                    baseStatValue = body.baseDamage + (body.levelDamage * body.level);
                    break;
            }
            float modStatValue = baseStatValue * GetGeneMultiplier(body, statType);
            return modStatValue - baseStatValue;
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

            foreach (GeneStat stat in Enum.GetValues(typeof(GeneStat)))
            {
                if (diffValues[stat] > 0)
                {
                    tokensToGive.Add(GeneTokens.tokenDict[stat][GeneMod.Plus1], (int)(diffValues[stat] * 100));
                }
                else
                {
                    tokensToGive.Add(GeneTokens.tokenDict[stat][GeneMod.Minus1], (int)(diffValues[stat] * -100));
                }
            }

            return tokensToGive;
        }
        #endregion
    }
}
