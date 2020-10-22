using RoR2;
using UnityEngine;

namespace GeneticsArtifact
{
    internal class GeneBehaviour : MonoBehaviour
    {
        internal GeneTracker tracker;
        internal CharacterBody body;
        internal float timePulse, timeAlive = 1f, damageDealt = 0f;
        internal static bool spawnLogging, accidentalDeathLogging;

        private void OnEnable()
        {
            body = gameObject.GetComponent<CharacterBody>();
            if (GeneticMasterController.trackerPerMonsterID)
            {
                tracker = new GeneTracker(gameObject.GetComponent<CharacterBody>().bodyIndex);
            }
            else
            {
                tracker = new GeneTracker(Random.Range(0, GeneticMasterController.masterTrackers.Count - 1));
            }
            GeneticMasterController.livingBehaviours.Add(this);
            ApplyMutation();
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

        private void ApplyMutation()
        {
            while (tracker.isLocked)
            {
                //Wait for the tracker to unlock itself before working
            }

            tracker.isLocked = true;

            //body.baseMaxHealth *= tracker.genes[0];
            //body.levelMaxHealth *= tracker.genes[0];

            //body.baseRegen *= tracker.genes[1];
            //body.levelRegen *= tracker.genes[1];

            //body.baseMoveSpeed *= tracker.genes[2];
            //body.levelMoveSpeed *= tracker.genes[2];

            //body.baseAcceleration *= tracker.genes[3];

            body.baseDamage *= tracker.genes[4];
            body.levelDamage *= tracker.genes[4];

            body.baseAttackSpeed *= tracker.genes[5];
            body.levelAttackSpeed *= tracker.genes[5];

            body.baseArmor *= tracker.genes[6];
            body.levelArmor *= tracker.genes[6];

            //If Size is enabled, apply it
            if (tracker.genes.Count == 8)
            {
                body.transform.localScale *= tracker.genes[7];
            }

            tracker.isLocked = false;

            body.RecalculateStats();

            if(spawnLogging || (accidentalDeathLogging && (body.maxHealth < 0f || float.IsNaN(body.maxHealth))))
            {
                if(body.healthComponent.health < 0f)
                {
                    GeneticsArtifactPlugin.geneticLogSource.LogWarning("Body spawned with " + tracker.GetGeneString() + "and Max Health = " + body.maxHealth.ToString() + " and Level = " + body.level.ToString());
                }
                else
                {
                    GeneticsArtifactPlugin.geneticLogSource.LogInfo("Body spawned with " + tracker.GetGeneString() + "and Max Health = " + body.maxHealth.ToString() + " and Level = " + body.level.ToString());
                }
            }
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