﻿using RoR2;
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

        //private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        //{
        //    orig(self, damageInfo);
        //    //Give tracker points equal to the damage it dealth
        //    if (damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>() == body)
        //    {
        //        damageDealt += damageInfo.damage;
        //    }
        //    else if (damageInfo.inflictor && damageInfo.inflictor.GetComponent<CharacterBody>() == body)
        //    {
        //        damageDealt += damageInfo.damage;
        //    }
        //}
    }
}