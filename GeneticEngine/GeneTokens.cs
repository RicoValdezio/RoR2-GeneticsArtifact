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
            tokenDict = new Dictionary<GeneStat, Dictionary<GeneMod, ItemDef>>();
            foreach (GeneStat stat in Enum.GetValues(typeof(GeneStat)))
            {
                tokenDict.Add(stat, new Dictionary<GeneMod, ItemDef>());
                foreach (GeneMod mod in Enum.GetValues(typeof(GeneMod)))
                {
                    ItemDef def = ScriptableObject.CreateInstance<ItemDef>();

                    def.name = "GENETOKEN_" + stat.ToString().ToUpper() + "_" + mod.ToString().ToUpper();
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
        Plus100,
        Plus25,
        Plus5,
        Plus1,
        Minus1,
        Minus5,
        Minus25
    }
}
