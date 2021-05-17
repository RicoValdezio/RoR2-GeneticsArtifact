using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticsArtifact
{
    public class GeneTokens
    {
        public static Dictionary<GeneStat, Dictionary<GeneMod, ItemDef>> tokenDict;

        public static void Init()
        {
            LanguageAPI.Add("GENETIC_EMPTY_TOKEN", "This is better than null I guess");
            tokenDict = new Dictionary<GeneStat, Dictionary<GeneMod, ItemDef>>();
            foreach (GeneStat stat in Enum.GetValues(typeof(GeneStat)))
            {
                tokenDict.Add(stat, new Dictionary<GeneMod, ItemDef>());
                foreach (GeneMod mod in Enum.GetValues(typeof(GeneMod)))
                {
                    ItemDef def = ScriptableObject.CreateInstance<ItemDef>();

                    def.name = "GENETOKEN_" + stat.ToString().ToUpper() + "_" + mod.ToString().ToUpper();
                    def.nameToken = "GENETIC_EMPTY_TOKEN";
                    def.pickupToken = "GENETIC_EMPTY_TOKEN";
                    def.descriptionToken = "GENETIC_EMPTY_TOKEN";
                    def.loreToken = "GENETIC_EMPTY_TOKEN";
                    def.pickupIconSprite = null;
                    def.pickupModelPrefab = null;
                    def.tags = new ItemTag[] { ItemTag.CannotCopy, ItemTag.CannotSteal };
                    def.tier = ItemTier.NoTier;
                    def.hidden = true;
                    def.canRemove = false;

                    CustomItem item = new CustomItem(def, new ItemDisplayRuleDict());
                    ItemAPI.Add(item);

                    tokenDict[stat].Add(mod, def);
                }
            }
        }
    }

    public enum GeneStat
    {
        MaxHealth,
        MoveSpeed,
        AttackSpeed,
        AttackDamage
    }

    public enum GeneMod
    {
        Plus1,
        Minus1
    }
}
