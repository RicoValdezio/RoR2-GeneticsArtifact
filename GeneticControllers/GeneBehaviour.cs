using RoR2;
using UnityEngine;

namespace GeneticsArtifact
{
    public class GeneBehaviour : MonoBehaviour
    {
        public GeneTracker tracker, masterTracker;
        public CharacterBody body;
        public float timePulse, timeAlive = 1f, damageDealt = 0f;

        private void OnEnable()
        {
            body = gameObject.GetComponent<CharacterBody>();
            if (ConfigMaster.trackerPerMonsterID.Value)
            {
                tracker = new GeneTracker(gameObject.GetComponent<CharacterBody>().bodyIndex);
                masterTracker = GeneticMasterController.masterTrackers.Find(x => x.index == tracker.index);
            }
            else
            {
                masterTracker = GeneticMasterController.masterTrackers[Random.Range(0, ConfigMaster.maxTrackers.Value - 1)];
                tracker = new GeneTracker(masterTracker.index);
            }
            GeneticMasterController.livingBehaviours.Add(this);
            ApplyMutation();

            if (ConfigMaster.spawnLogging.Value || (ConfigMaster.accidentalDeathLogging.Value && (body.maxHealth < 0f || float.IsNaN(body.maxHealth))))
            {
                if (body.healthComponent.health < 0f)
                {
                    GeneticsArtifactPlugin.geneticLogSource.LogWarning(tracker.BuildGenePairMessage());
                }
                else
                {
                    GeneticsArtifactPlugin.geneticLogSource.LogInfo(tracker.BuildGenePairMessage());
                }
            }
        }

        private void OnDisable()
        {
            //Calculate the score
            tracker.score = damageDealt / timeAlive;
            //If the stage ends, add body to dead list
            GeneticMasterController.deadTrackers.Add(tracker);
            //And destroy self and deref tracker
            tracker.Dispose();
            GeneticMasterController.livingBehaviours.Remove(this);
            Destroy(this);
        }

        public void ApplyMutation()
        {
            while (tracker.isLocked)
            {
                //Wait for the tracker to unlock itself before working
            }

            tracker.isLocked = true;

            //If Size is enabled, apply it
            if (tracker.GetGeneValue("Size") is float sizeMultiplier)
            {
                body.transform.localScale *= sizeMultiplier;
            }

            tracker.isLocked = false;

            body.RecalculateStats();

            
        }

        private void Update()
        {
            //Every second its alive, give it a point
            timePulse += Time.deltaTime;
            if (timePulse >= 1f)
            {
                timeAlive += timePulse;
                timePulse = 0f;
            }
        }
    }
}