using UnityEngine;

namespace GeneticsArtifact
{
    public enum GeneBalanceType
    {
        Ignored,
        Normal,
        Centered,
        Inverted
    }

    public class GenePair
    {
        public string name;
        public float value;
        public float maxValue, minValue, mutValue, penValue;
        public GeneBalanceType balanceType;

        public GenePair(string givenName, float givenValue, GeneBalanceType givenType, float givenMax, float givenMin, float givenMutationScale, float givenPenaltySize)
        {
            name = givenName;
            value = givenValue;
            balanceType = givenType;
            maxValue = givenMax;
            minValue = givenMin;
            mutValue = givenMutationScale;
            penValue = givenPenaltySize;
        }

        public void Mutate()
        {
            value = Mathf.Clamp(Random.Range(value * (1 - mutValue), value * (1 + mutValue)), minValue, maxValue);
        }

        public void MutateXTimes(int timesToMutate)
        {
            for(int x = 0; x < timesToMutate; x++)
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
                case GeneBalanceType.Inverted:
                    return 1f / value;
                default: //GeneBalanceType.Ignored
                    return 1f;
            }
        }

        public void ApplyBalancePenalty()
        {
            switch (balanceType)
            {
                case GeneBalanceType.Normal:
                    value = Mathf.Max(minValue, value - penValue);
                    break;
                case GeneBalanceType.Centered:
                    if(value > 1f)
                    {
                        value = Mathf.Max(1f, value - penValue);
                    }
                    else
                    {
                        value = Mathf.Min(1f, value + penValue);
                    }
                    break;
                case GeneBalanceType.Inverted:
                    value = Mathf.Min(maxValue, value + penValue);
                    break;
                default: //GeneBalanceType.Ignored
                    break;
            }
        }
    }
}