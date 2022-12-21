using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GeneticsArtifact
{
    public class MonsterGeneBehaviour : MonoBehaviour
    {
        public BodyIndex bodyIndex;
        public Dictionary<GeneStat, float> currentGenes;
        public CharacterBody characterBody;
        public float timeAlive = 0f, timeEngaged = 0f, damageDealt = 0f, score = 0f;
        public event EventHandler MoGBPostMutationEvent, MoGBPostScoringEvent;

        public void Awake()
        {
            characterBody = gameObject.GetComponent<CharacterBody>();
            bodyIndex = characterBody.bodyIndex;
            InitializeCurrentGenes();
            GeneEngineDriver.livingGenes.Add(this);
        }

        public void Update()
        {
            timeAlive += Time.deltaTime;
            if (!characterBody.outOfCombat) timeEngaged += Time.deltaTime;
        }

        #region Mutation
        /// <summary>
        /// Grabs a copy of the master gene template and mutates itself once
        /// </summary>
        public void MutateFromMaster()
        {
            CopyFromMaster();
            MutateSelf();
        }

        /// <summary>
        /// Grabs a copy of the master genes without mutating
        /// </summary>
        public void CopyFromMaster()
        {
            MasterGeneBehaviour master = GeneEngineDriver.masterGenes.Find(x => x.bodyIndex == bodyIndex);
            AdaptToNewGenes(master.templateGenes);
        }

        /// <summary>
        /// Mutates its genes once, and applies penalties if overmutated
        /// </summary>
        public void MutateSelf()
        {
            Dictionary<GeneStat, float> mutationAttempt = GenerateMutationAttempt();
            mutationAttempt = CorrectOvermutation(mutationAttempt);
            AdaptToNewGenes(mutationAttempt);
            MoGBPostMutationEvent?.Invoke(this, new EventArgs());
        }

        private Dictionary<GeneStat, float> CorrectOvermutation(Dictionary<GeneStat, float> attempt)
        {
            Dictionary<GeneStat, float> correction = attempt;
            while (CalculateGeneProduct(correction) > ConfigManager.geneProductLimit.Value)
            {
                correction[correction.Aggregate((x, y) => x.Value > y.Value ? x : y).Key] -= 0.05f;
            }
            return correction;
        }

        private float CalculateGeneProduct(Dictionary<GeneStat, float> testValues)
        {
            float product = 1f;
            foreach (float value in testValues.Values) product *= value;
            return product;
        }

        public void LogDebugInfo()
        {
            GeneticsArtifactPlugin.geneticLogSource.LogInfo(Stage.instance.sceneDef.baseSceneName + " " +
                                                            characterBody.name + " " +
                                                            currentGenes[GeneStat.MaxHealth].ToString() + " " +
                                                            currentGenes[GeneStat.MoveSpeed].ToString() + " " +
                                                            currentGenes[GeneStat.AttackSpeed].ToString() + " " +
                                                            currentGenes[GeneStat.AttackDamage].ToString());
        }

        private void InitializeCurrentGenes()
        {
            currentGenes = new Dictionary<GeneStat, float>();
            foreach (GeneStat stat in Enum.GetValues(typeof(GeneStat)))
            {
                currentGenes.Add(stat, 1f);
            }
        }

        private void AdaptToNewGenes(Dictionary<GeneStat, float> newGenes)
        {
            Dictionary<ItemDef, int> itemsToGive = GeneTokenCalc.GetTokensToAdd(currentGenes, newGenes);
            foreach (KeyValuePair<ItemDef, int> pair in itemsToGive)
            {
                characterBody.inventory.GiveItem(pair.Key, pair.Value);
            }
#if DEBUG
            GeneticsArtifactPlugin.geneticLogSource.LogInfo(Stage.instance.sceneDef.baseSceneName + " " +
                                                            characterBody.name + Environment.NewLine +
                                                            "Old Genes: " +
                                                            currentGenes[GeneStat.MaxHealth].ToString() + " " +
                                                            currentGenes[GeneStat.MoveSpeed].ToString() + " " +
                                                            currentGenes[GeneStat.AttackSpeed].ToString() + " " +
                                                            currentGenes[GeneStat.AttackDamage].ToString() + Environment.NewLine +
                                                            "New Genes: " +
                                                            newGenes[GeneStat.MaxHealth].ToString() + " " +
                                                            newGenes[GeneStat.MoveSpeed].ToString() + " " +
                                                            newGenes[GeneStat.AttackSpeed].ToString() + " " +
                                                            newGenes[GeneStat.AttackDamage].ToString());
#endif
            currentGenes = newGenes;
        }

        private Dictionary<GeneStat, float> GenerateMutationAttempt()
        {
            Dictionary<GeneStat, float> mutationAttempt = new Dictionary<GeneStat, float>();
            float diffScalar = ConfigManager.geneVarianceLimit.Value * (Stage.instance.sceneDef.baseSceneName == "artifactworld" ? 5 : 1);

            foreach (GeneStat stat in currentGenes.Keys)
            {
                float currentValue = currentGenes[stat];
                if(ConfigManager.enableGeneLimitOverrides.Value && GeneEngineDriver.geneLimitOverrides.ContainsKey(stat))
                {
                    mutationAttempt.Add(stat, (float)decimal.Round((decimal)Mathf.Clamp(UnityEngine.Random.Range(currentValue * (1 - diffScalar), currentValue * (1 + diffScalar)),
                                                                                        GeneEngineDriver.geneLimitOverrides[stat].Item1, GeneEngineDriver.geneLimitOverrides[stat].Item2)));
                }
                else
                {
                    mutationAttempt.Add(stat, (float)decimal.Round((decimal)Mathf.Clamp(UnityEngine.Random.Range(currentValue * (1 - diffScalar), currentValue * (1 + diffScalar)),
                                                                                        ConfigManager.geneFloor.Value, ConfigManager.geneCap.Value)));
                }
            }
            return mutationAttempt;
        }
        #endregion

        #region EndOfLife
        private void OnDestroy()
        {
            ScoreMe();
            GeneEngineDriver.livingGenes.Remove(this);
            GeneEngineDriver.deadGenes.Add(this);
        }

        /// <summary>
        /// Calculates its current score
        /// </summary>
        /// <returns>The current score</returns>
        public float ScoreMe()
        {
            score = 0f;
            //Score is the total damage dealt multiplied the the percent of time spent in combat (this isn't DPS)
            if (timeAlive > 0f && timeEngaged > 0f)
            {
                float engageQuotient = timeEngaged / timeAlive;
                score = damageDealt * engageQuotient;
            }
            MoGBPostScoringEvent?.Invoke(this, new EventArgs());
            return score;
        }
        #endregion
    }
}
