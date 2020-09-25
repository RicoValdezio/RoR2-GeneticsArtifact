using RoR2;
using UnityEngine;

namespace GeneticsArtifact
{
    internal class GeneBehaviour : MonoBehaviour
    {
        internal GeneTracker tracker;
        internal CharacterBody body;
        internal float timePulse, timeAlive = 0f, damageDealt = 0f;

        private void OnEnable()
        {
            body = gameObject.GetComponent<CharacterBody>();
        }

        private void Start()
        {
            if (GeneticMasterController.trackerPerMonsterID)
            {
                tracker = new GeneTracker(gameObject.GetComponent<CharacterBody>().bodyIndex);
            }
            else
            {
                tracker = new GeneTracker(Random.Range(0, GeneticMasterController.masterTrackers.Count - 1));
            }
            ApplyMutation();

            //On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.CharacterBody.Update += CharacterBody_Update;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void OnDisable()
        {
            //Calculate the score
            tracker.score = damageDealt / timeAlive;
            //If the stage ends, add body to dead list
            GeneticMasterController.deadTrackers.Add(tracker);
            //And destroy self and deref tracker
            tracker.Dispose();
            Destroy(this);
        }

        private void ApplyMutation()
        {
            body.baseMaxHealth *= tracker.genes[0];
            body.levelMaxHealth *= tracker.genes[0];

            body.baseRegen *= tracker.genes[1];
            body.levelRegen *= tracker.genes[1];

            body.baseMoveSpeed *= tracker.genes[2];
            body.levelMoveSpeed *= tracker.genes[2];

            body.baseAcceleration *= tracker.genes[3];

            body.baseDamage *= tracker.genes[4];
            body.levelDamage *= tracker.genes[4];

            body.baseAttackSpeed *= tracker.genes[5];
            body.levelAttackSpeed *= tracker.genes[5];

            body.baseArmor *= tracker.genes[6];
            body.levelArmor *= tracker.genes[6];

            body.RecalculateStats();
        }

        //Removed due to OnDisable being triggered by the body death sequence and this causing duplicate copies
        //private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        //{
        //    orig(self, damageReport);
        //    //If body is dead, add to the dead list
        //    if(damageReport.victimBody == body)
        //    {
        //        //GeneticMasterController.deadTrackers.Add(tracker);
        //        //Chat.AddMessage("Death of a " + body.baseNameToken + " with " + tracker.score.ToString() + " points.");
        //        enabled = false;
        //    }
        //}

        private void CharacterBody_Update(On.RoR2.CharacterBody.orig_Update orig, CharacterBody self)
        {

            orig(self);
            //Every second its alive, give it a point
            if (self == body)
            {
                timePulse += Time.deltaTime;
                if (timePulse >= 1f)
                {
                    timeAlive += timePulse;
                    timePulse = 0f;
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            //Give tracker points equal to the damage it dealth
            if (damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>() == body)
            {
                damageDealt += damageInfo.damage;
            }
            else if (damageInfo.inflictor && damageInfo.inflictor.GetComponent<CharacterBody>() == body)
            {
                damageDealt += damageInfo.damage;
            }
        }
    }
}