﻿using UnityEngine;

namespace GeneticsArtifact
{
    public enum GeneBalanceType
    {
        Ignored,
        Normal,
        Centered,
    }

    public class GenePair
    {
        public string name;
        public float value;
        public float maxValue, minValue, mutValue;
        public GeneBalanceType balanceType;

        public GenePair(string givenName, float givenValue, GeneBalanceType givenType, float givenMax, float givenMin, float givenMutStep)
        {
            name = givenName;
            value = givenValue;
            balanceType = givenType;
            maxValue = givenMax;
            minValue = givenMin;
            mutValue = givenMutStep;
        }

        public void Mutate()
        {
            value = Mathf.Clamp(Random.Range(value * (1 - mutValue), value * (1 + mutValue)), minValue, maxValue);
        }

        public void MutateXTimes(int timesToMutate)
        {
            for(int x = 1; x < timesToMutate; x++)
            {
                Mutate();
            }
        }

        public float GetBalanceValue()
        {
            switch (balanceType)
            {
                case GeneBalanceType.Normal:
                    return value;
                case GeneBalanceType.Centered:
                    return Mathf.Max(value / 1f, 1f / value);
                default: //GeneBalanceType.Ignored
                    return 1f;
            }
        }
    }
}