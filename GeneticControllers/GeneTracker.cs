using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GeneticsArtifact
{
    public class GeneTracker : IDisposable
    {
        public int index;
        public GeneTracker masterTracker;

        public List<GenePair> genePairs;
        public float score = 0f;
        public bool isLocked = false;

        public static float absoluteFloor, absoluteCeil, deviationFromParent, balanceLimit, balanceStep;
        public static bool useSizeModifier;
        private bool disposedValue;

        public GeneTracker(int refIndex, bool isMaster = false)
        {
            index = refIndex;
            genePairs = new List<GenePair>();
            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def.artifactIndex))
            {
                absoluteFloor = 1f / absoluteCeil;
                genePairs.AddRange(new List<GenePair>
                {
                    new GenePair("Health", 1f, GeneBalanceType.Normal, absoluteCeil, absoluteFloor, deviationFromParent, balanceStep),
                    new GenePair("Regen", 1f, GeneBalanceType.Normal, absoluteCeil, absoluteFloor, deviationFromParent, balanceStep),
                    new GenePair("MoveSpeed", 1f, GeneBalanceType.Normal, absoluteCeil, absoluteFloor, deviationFromParent, balanceStep),
                    new GenePair("Damage", 1f, GeneBalanceType.Normal, absoluteCeil, absoluteFloor, deviationFromParent, balanceStep),
                    new GenePair("AttackSpeed", 1f, GeneBalanceType.Normal, absoluteCeil, absoluteFloor, deviationFromParent, balanceStep),
                    new GenePair("Armor", 1f, GeneBalanceType.Normal, absoluteCeil, absoluteFloor, deviationFromParent, balanceStep)
                });
                if (useSizeModifier)
                {
                    genePairs.Add(new GenePair("Size", 1f, GeneBalanceType.Centered, absoluteCeil, absoluteFloor, deviationFromParent, balanceStep));
                }
            }
            //If not a master, get values from a master
            if (!isMaster)
            {
                masterTracker = GeneticMasterController.masterTrackers.Find(x => x.index == index);
                MutateFromParent();
            }
        }

        //I will need to deprecate this eventually
        public float GetGeneValue(string compareName)
        {
            //If the gene exists, return its value
            if (genePairs.Any(x => x.name == compareName))
            {
                return genePairs.Find(x => x.name == compareName).value;
            }
            else //Return the default value of 1
            {
                return 1f;
            }
        }

        //Same for this, as the generalization will make this useless
        public void SetGeneValue(string compareName, float newValue)
        {
            //If the gene exists, return its value
            if (genePairs.Any(x => x.name == compareName))
            {
                genePairs.Find(x => x.name == compareName).value = newValue;
            }
            else //Throw an exception as if it were a nullref
            {
                throw new NullReferenceException("Tracker has no GenePair with name = " + compareName);
            }
        }

        public void MutateFromParent()
        {
            //Wait until the master is free
            while (masterTracker.isLocked)
            {
                //Do nothing and just wait
            }
            //Lock the master and self
            masterTracker.isLocked = true;
            isLocked = true;

            foreach (GenePair gene in genePairs)
            {
                gene.Mutate();
            }

            //Unlock the master and apply balance, then unlock self
            masterTracker.isLocked = false;
            ApplyNewBalanceSystem();
            isLocked = false;
        }

        public void ApplyNewBalanceSystem()
        {
            //Start applying penalties until below the balanceLimit
            while (DetermineCurrentBalance() > balanceLimit)
            {
                //This is optimistic as it assumes that there is a high chance of hitting a decrease-able gene
                genePairs[UnityEngine.Random.Range(0, genePairs.Count - 1)].ApplyBalancePenalty();
            }
        }

        public float DetermineCurrentBalance()
        {
            float currentBalance = 1f;
            foreach (GenePair gene in genePairs)
            {
                currentBalance *= gene.GetBalanceValue();
            }
            return currentBalance;
        }

        public void MutateFromChildren()
        {
            List<GenePair> sumPairs = new List<GenePair>();
            float totalScore = 0f;
            //Lock self to prevent competiton with children
            while (isLocked)
            {
                //Do nothing
            }
            isLocked = true;
            //Use a modified weighted average system to determine mutation
            foreach (GeneTracker childTracker in GeneticMasterController.deadTrackers.Where(x => x.index == index))
            {
                if (!float.IsNaN(childTracker.score) && childTracker.score > 0f)
                {
                    foreach (GenePair childPair in childTracker.genePairs)
                    {
                        if (!sumPairs.Any(x => x.name == childPair.name))
                        {
                            sumPairs.Add(new GenePair(childPair.name, 0f, GeneBalanceType.Ignored, 0f, 0f, 0f, 0f));
                        }
                        sumPairs.Find(x => x.name == childPair.name).value += childPair.value * childTracker.score;
                    }
                    totalScore += childTracker.score;
                }
            }
            //The actual averaging to determine new values
            if (!float.IsNaN(totalScore) && totalScore > 0)
            {
                foreach(GenePair genePair in sumPairs)
                {
                    SetGeneValue(genePair.name, genePair.value / totalScore);
                }
            }
            //Unlock self to allow spawning
            isLocked = false;
        }

        public string GetGeneString()
        {
            string message = "ID " + index.ToString("D2") + " | "
                    + GetGeneValue("Health").ToString("N4") + " | "
                    + GetGeneValue("Regen").ToString("N4") + " | "
                    + GetGeneValue("MoveSpeed").ToString("N4") + " | "
                    + GetGeneValue("Acceleration").ToString("N4") + " | "
                    + GetGeneValue("Damage").ToString("N4") + " | "
                    + GetGeneValue("AttackSpeed").ToString("N4") + " | "
                    + GetGeneValue("Armor").ToString("N4");
            return message;
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