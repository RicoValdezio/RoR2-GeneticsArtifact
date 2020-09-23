using EntityStates.BrotherMonster;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GeneticsArtifact
{
    internal class GeneTracker
    {
        internal int index;
        internal GeneTracker masterTracker;

        internal float healthMultiplier = 1f,
            regenMultiplier = 1f,
            moveSpeedMultiplier = 1f,
            accelMultiplier = 1f,
            damageMultiplier = 1f,
            attackSpeedMultiplier = 1f,
            armorMultiplier = 1f,
            sizeMultiplier = 1f,
            score = 0f;

        //Config these later
        internal static float absoluteFloor = 0.2f, absoluteCeil = 5f, relativeFloor = 0.9f, relativeCeil = 1.1f;

        public GeneTracker(int refIndex, bool isMaster = false)
        {
            index = refIndex;
            //If not a master, get values from a master
            if (!isMaster)
            {
                masterTracker = GeneticMasterController.masterTrackers.Find(x => x.index == index);
                MutateFromParent();
            }
        }

        private void MutateFromParent()
        {
            float tempValue;
            #region Size Health and MoveSpeed/Accel
            //Start by calculating size, since health and moveSpeed are tied to it
            tempValue = masterTracker.sizeMultiplier *= Random.Range(relativeFloor, relativeCeil);
            if(absoluteFloor <= tempValue && tempValue <= absoluteCeil)
            {
                sizeMultiplier = tempValue;
            }
            else if(absoluteFloor > tempValue)
            {
                sizeMultiplier = absoluteFloor;
            }
            else
            {
                sizeMultiplier = absoluteCeil;
            }
            //Calculate health using the size modifer
            tempValue = sizeMultiplier *= Random.Range(relativeFloor, relativeCeil);
            if (absoluteFloor <= tempValue && tempValue <= absoluteCeil)
            {
                healthMultiplier = tempValue;
            }
            else if (absoluteFloor > tempValue)
            {
                healthMultiplier = absoluteFloor;
            }
            else
            {
                healthMultiplier = absoluteCeil;
            }
            //Calculate moveSpeed and accel using size
            tempValue = sizeMultiplier *= Random.Range(relativeFloor, relativeCeil);
            if (absoluteFloor <= tempValue && tempValue <= absoluteCeil)
            {
                moveSpeedMultiplier = tempValue;
                accelMultiplier = tempValue;
            }
            else if (absoluteFloor > tempValue)
            {
                moveSpeedMultiplier = absoluteFloor;
                accelMultiplier = absoluteFloor;
            }
            else
            {
                moveSpeedMultiplier = absoluteCeil;
                accelMultiplier = absoluteCeil;
            }
            #endregion
            #region Armor and Regen
            //Armor and Regen work against eachother, higher armor means lower regen
            tempValue = masterTracker.armorMultiplier *= Random.Range(relativeFloor, relativeCeil);
            if (absoluteFloor <= tempValue && tempValue <= absoluteCeil)
            {
                armorMultiplier = tempValue;
            }
            else if (absoluteFloor > tempValue)
            {
                armorMultiplier = absoluteFloor;
            }
            else
            {
                armorMultiplier = absoluteCeil;
            }
            regenMultiplier = 1f / armorMultiplier;
            #endregion
            #region AttackSpeed and Damage
            //AttackSpeed and Damage work against eachother, higher damage means lower attack speed
            tempValue = masterTracker.damageMultiplier *= Random.Range(relativeFloor, relativeCeil);
            if (absoluteFloor <= tempValue && tempValue <= absoluteCeil)
            {
                damageMultiplier = tempValue;
            }
            else if (absoluteFloor > tempValue)
            {
                damageMultiplier = absoluteFloor;
            }
            else
            {
                damageMultiplier = absoluteCeil;
            }
            attackSpeedMultiplier = 1f / damageMultiplier;
            #endregion
        }

        internal void MutateFromChildren()
        {
            float healthWeight = 0f,
                regenWeight = 0f,
                moveSpeedWeight = 0f,
                accelWeight = 0f,
                damageWeight = 0f,
                attackSpeedWeight = 0f,
                armorWeight = 0f,
                sizeWeight = 0f,
                scoreWeight = 0f;
            //Use a modified weighted average to update master
            foreach(GeneTracker childTracker in GeneticMasterController.deadTrackers.Where(x => x.index == index))
            {
                healthWeight += childTracker.healthMultiplier * childTracker.score;
                regenWeight += childTracker.regenMultiplier * childTracker.score;
                moveSpeedWeight += childTracker.moveSpeedMultiplier * childTracker.score;
                accelWeight += childTracker.accelMultiplier * childTracker.score;
                damageWeight += childTracker.damageMultiplier * childTracker.score;
                attackSpeedWeight += childTracker.attackSpeedMultiplier * childTracker.score;
                armorWeight += childTracker.armorMultiplier * childTracker.score;
                sizeWeight += childTracker.sizeMultiplier * childTracker.score;
                scoreWeight +=  childTracker.score;
            }
            if(scoreWeight > 0)
            {
                healthMultiplier = healthWeight / scoreWeight;
                regenMultiplier = regenWeight / scoreWeight;
                moveSpeedMultiplier = moveSpeedWeight / scoreWeight;
                accelMultiplier = accelWeight / scoreWeight;
                damageMultiplier = damageWeight / scoreWeight;
                attackSpeedMultiplier = attackSpeedWeight / scoreWeight;
                armorMultiplier = armorWeight / scoreWeight;
                sizeMultiplier = sizeWeight / scoreWeight;
            }
        }
    }
}