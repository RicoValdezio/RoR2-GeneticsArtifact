using UnityEngine;

namespace GeneticsArtifact
{
    public enum GeneBalanceType
    {
        /// <summary>
        /// Gene is not used in the balance system
        /// </summary>
        Ignored,
        /// <summary>
        /// Balance penalty reduces the value if possible
        /// </summary>
        Normal,
        /// <summary>
        /// Balance penalty reduces the value if greater than 1, and increases it if less than 1
        /// </summary>
        Centered,
        /// <summary>
        /// Balance penalty increases the value if possible
        /// </summary>
        Inverted
    }

    public class GenePair
    {
        public string name;
        public float value;
        public float maxValue, minValue, mutValue, penValue;
        public GeneBalanceType balanceType;

        public GenePair(string givenName, float givenValue, GeneBalanceType givenType, float givenMax, float givenMin)
        {
            name = givenName;
            value = givenValue;
            balanceType = givenType;
            maxValue = givenMax;
            minValue = givenMin;
            mutValue = ConfigMaster.deviationFromParent;
            penValue = ConfigMaster.balanceStep;
        }

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

        /// <summary>
        /// The standard mutate method, value will shift by up to mutValue, and is bound by minValue and maxValue
        /// </summary>
        public void Mutate()
        {
            value = Mathf.Clamp(Random.Range(value * (1 - mutValue), value * (1 + mutValue)), minValue, maxValue);
        }

        /// <summary>
        /// Variation of standard method, value will shift by up to mutValue, but is not bound by minValue or maxValue
        /// </summary>
        public void UnclampedMutate()
        {
            value = Random.Range(value * (1 - mutValue), value * (1 + mutValue));
        }

        /// <summary>
        /// Variation of standard method, value is only bound by minValue and maxValue
        /// </summary>
        public void MinMaxMutate()
        {
            value = Random.Range(minValue, maxValue);
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

        public string GetNameValueString()
        {
            return name + " " + value.ToString("N4");
        }
    }
}