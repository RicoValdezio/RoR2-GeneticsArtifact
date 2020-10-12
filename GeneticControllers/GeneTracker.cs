﻿using IL.RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace GeneticsArtifact
{
    internal class GeneTracker : IDisposable
    {
        internal int index;
        internal GeneTracker masterTracker;

        //Order is Health, Regen, MoveSpeed, Accel, Damage, AttackSpeed, Armor, (Size was removed)
        internal List<float> genes = new List<float>();
        internal float score = 0f;

        internal static float absoluteFloor, absoluteCeil, relativeFloor, relativeCeil, deviationFromParent, balanceLimit, balanceStep;
        internal static bool useSizeModifier;
        private bool disposedValue;

        public GeneTracker(int refIndex, bool isMaster = false)
        {
            index = refIndex;
            for (int i = 0; i < 7; i++)
            {
                genes.Add(1);
            }
            if (useSizeModifier)
            {
                genes.Add(1);
            }
            //If not a master, get values from a master
            if (!isMaster)
            {
                masterTracker = GeneticMasterController.masterTrackers.Find(x => x.index == index);
                MutateFromParent();
            }
            relativeCeil = 1f + deviationFromParent;
            relativeFloor = 1f - deviationFromParent;
            absoluteFloor = 1f / absoluteCeil;
        }

        private void MutateFromParent()
        {
            float tempValue;
            for (int i = 0; i < genes.Count; i++)
            {
                tempValue = masterTracker.genes[i] * UnityEngine.Random.Range(relativeFloor, relativeCeil);
                //Check if inside absolute bounds
                if (tempValue > absoluteCeil)
                {
                    tempValue = absoluteCeil;
                }
                else if (tempValue < absoluteFloor)
                {
                    tempValue = absoluteFloor;
                }
                //Apply the change
                genes[i] = tempValue;
            }
            ApplyNewBalanceSystem();
        }

        private void ApplyNewBalanceSystem()
        {
            int statToDecrease;
            //Start applying penalties until below the balanceLimit
            while(DetermineCurrentBalance() > balanceLimit)
            {
                //This is unsafe as it assumes that the chances of hitting a stat that CAN decrease is almost certain
                statToDecrease = UnityEngine.Random.Range(0, 6);
                if(genes[statToDecrease] - balanceStep >= absoluteFloor)
                {
                    genes[statToDecrease] -= balanceStep;
                }
            }
        }

        private float DetermineCurrentBalance()
        {
            //Only the first 7 stats count towards balance, size is just for fun
            float currentBalance = 1f;
            for (int i = 0; i < 7; i++)
            {
                currentBalance *= genes[i];
            }
            return currentBalance;
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
                scoreWeight = 0f;
            //Use a modified weighted average to update master
            foreach (GeneTracker childTracker in GeneticMasterController.deadTrackers.Where(x => x.index == index))
            {
                healthWeight += childTracker.genes[0] * childTracker.score;
                regenWeight += childTracker.genes[1] * childTracker.score;
                moveSpeedWeight += childTracker.genes[2] * childTracker.score;
                accelWeight += childTracker.genes[3] * childTracker.score;
                damageWeight += childTracker.genes[4] * childTracker.score;
                attackSpeedWeight += childTracker.genes[5] * childTracker.score;
                armorWeight += childTracker.genes[6] * childTracker.score;
                scoreWeight += childTracker.score;
            }
            if (scoreWeight > 0)
            {
                genes[0] = healthWeight / scoreWeight;
                genes[1] = regenWeight / scoreWeight;
                genes[2] = moveSpeedWeight / scoreWeight;
                genes[3] = accelWeight / scoreWeight;
                genes[4] = damageWeight / scoreWeight;
                genes[5] = attackSpeedWeight / scoreWeight;
                genes[6] = armorWeight / scoreWeight;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GeneTracker()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}