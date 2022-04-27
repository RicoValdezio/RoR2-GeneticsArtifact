using RoR2;
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

        public void Awake()
        {
            characterBody = gameObject.GetComponent<CharacterBody>();
            bodyIndex = characterBody.bodyIndex;
            currentGenes = new Dictionary<GeneStat, float>();
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

        private void CopyFromMaster()
        {
            MasterGeneBehaviour master = GeneEngineDriver.masterGenes.Find(x => x.bodyIndex == bodyIndex);
            currentGenes = new Dictionary<GeneStat, float>(master.templateGenes);
        }

        /// <summary>
        /// Mutates its genes once, and applies penalties if overmutated
        /// </summary>
        public void MutateSelf()
        {
            Dictionary<GeneStat, float> mutationAttempt = new Dictionary<GeneStat, float>();
            foreach (GeneStat stat in currentGenes.Keys)
            {
                //Bulwark mutation is 20%-500% - base is 90%-110%
                mutationAttempt.Add(stat, Stage.instance.sceneDef.baseSceneName == "artifactworld" ?
                                          (float)decimal.Round((decimal)Mathf.Clamp(Random.Range(currentGenes[stat] * (1 - ConfigManager.geneVarianceLimit.Value * 5), 
                                                                                                 currentGenes[stat] * (1 + ConfigManager.geneVarianceLimit.Value * 5)), 
                                                                                    ConfigManager.geneFloor.Value, ConfigManager.geneCap.Value), 2) :
                                          (float)decimal.Round((decimal)Mathf.Clamp(Random.Range(currentGenes[stat] * (1 - ConfigManager.geneVarianceLimit.Value), 
                                                                                                 currentGenes[stat] * (1 + ConfigManager.geneVarianceLimit.Value)), 
                                                                                    ConfigManager.geneFloor.Value, ConfigManager.geneCap.Value), 2));
            }
            mutationAttempt = CorrectOvermutation(mutationAttempt);
            Dictionary<ItemDef, int> itemsToGive = GeneTokenCalc.GetTokensToAdd(currentGenes, mutationAttempt);
            foreach (KeyValuePair<ItemDef, int> pair in itemsToGive)
            {
                characterBody.inventory.GiveItem(pair.Key, pair.Value);
            }
            currentGenes = mutationAttempt;
#if DEBUG
            GeneticsArtifactPlugin.geneticLogSource.LogInfo(Stage.instance.sceneDef.baseSceneName + " " +
                                                            characterBody.name + " " +
                                                            currentGenes[GeneStat.MaxHealth].ToString() + " " + 
                                                            currentGenes[GeneStat.MoveSpeed].ToString() + " " + 
                                                            currentGenes[GeneStat.AttackSpeed].ToString() + " " + 
                                                            currentGenes[GeneStat.AttackDamage].ToString());
#endif
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
            return score;
        }
        #endregion
    }
}
