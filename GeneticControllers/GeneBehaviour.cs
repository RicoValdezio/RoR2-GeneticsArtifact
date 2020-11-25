using RoR2;
using UnityEngine;

namespace GeneticsArtifact
{
    public class GeneBehaviour : MonoBehaviour
    {
        public GeneTracker tracker;
        public CharacterBody body;
        public float timePulse, timeAlive = 1f, damageDealt = 0f;
        public static bool spawnLogging, accidentalDeathLogging;

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

            if(spawnLogging || (accidentalDeathLogging && (body.maxHealth < 0f || float.IsNaN(body.maxHealth))))
            {
                if (body.healthComponent.health < 0f)
                {
                    GeneticsArtifactPlugin.geneticLogSource.LogWarning(GetGeneString());
                }
                else
                {
                    GeneticsArtifactPlugin.geneticLogSource.LogInfo(GetGeneString());
                }
            }
        }

        public string GetGeneString()
        {
            string message = "Body spawned with ID " + tracker.index.ToString("D2") + " | "
                    + "Health " + tracker.GetGeneValue("Health").ToString("N4") + " " + body.maxHealth.ToString("N4") + " | "
                    + "Regen " + tracker.GetGeneValue("Regen").ToString("N4") + " " + body.regen.ToString("N4") + " | "
                    + "MoveSpeed " + tracker.GetGeneValue("MoveSpeed").ToString("N4") + " " + body.moveSpeed.ToString("N4") + " | "
                    + "Damage " + tracker.GetGeneValue("Damage").ToString("N4") + " " + body.damage.ToString("N4") + " | "
                    + "AttackSpeed " + tracker.GetGeneValue("AttackSpeed").ToString("N4") + " " + body.attackSpeed.ToString("N4") + " | "
                    + "Armor " + tracker.GetGeneValue("Armor").ToString("N4") + " " + body.armor.ToString("N4") + " | "
                    + "Level " + body.level.ToString();
            return message;
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