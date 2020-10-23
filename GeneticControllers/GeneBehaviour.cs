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

            //If Size is enabled, apply it
            if (tracker.genes.Count == 8)
            {
                body.transform.localScale *= tracker.genes[7];
            }

            tracker.isLocked = false;

            body.RecalculateStats();

            if(spawnLogging || (accidentalDeathLogging && (body.maxHealth < 0f || float.IsNaN(body.maxHealth))))
            {
                string message = "Body spawned with " + tracker.GetGeneString() + "and the following expression "
                    + body.maxHealth.ToString("N4") + " "
                    + body.regen.ToString("N4") + " "
                    + body.moveSpeed.ToString("N4") + " "
                    + body.acceleration.ToString("N4") + " "
                    + body.damage.ToString("N4") + " "
                    + body.attackSpeed.ToString("N4") + " "
                    + body.armor.ToString("N4") + " "
                    + "and Level = " + body.level.ToString();
                if (body.healthComponent.health < 0f)
                {
                    GeneticsArtifactPlugin.geneticLogSource.LogWarning(message);
                }
                else
                {
                    GeneticsArtifactPlugin.geneticLogSource.LogInfo(message);
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