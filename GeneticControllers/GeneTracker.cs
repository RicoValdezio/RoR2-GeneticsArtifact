using System.Collections.Generic;
using UnityEngine;

namespace GeneticsArtifact
{
    internal class GeneTracker
    {
        internal int index;
        private GeneTracker masterTracker;

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
                GeneTracker masterTracker = GeneticMasterController.masterTrackers.Find(x => x.index == index);
                MutateFromParent();
            }
        }

        private void MutateFromParent()
        {
            float tempValue;
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

        }
    }
}