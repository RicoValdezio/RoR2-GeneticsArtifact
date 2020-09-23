using RoR2;
using UnityEngine;

namespace GeneticsArtifact
{
    internal class GeneBehaviour : MonoBehaviour
    {
        internal GeneTracker tracker;
        internal CharacterBody body;
        internal float timePulse;

        private void OnEnable()
        {
            if (GeneticMasterController.trackerPerMonsterID)
            {
                tracker = new GeneTracker(gameObject.GetComponent<CharacterBody>().bodyIndex);
            }
            else
            {
                tracker = new GeneTracker(Random.Range(0, GeneticMasterController.masterTrackers.Count - 1));
            }
            body = gameObject.GetComponent<CharacterBody>();
            ApplyMutation();
            
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.CharacterBody.Update += CharacterBody_Update;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void OnDisable()
        {
            //If the stage ends, add body to dead list
            GeneticMasterController.deadTrackers.Add(tracker);
        }

        private void ApplyMutation()
        {
            body.baseMaxHealth *= tracker.healthMultiplier;
            body.levelMaxHealth *= tracker.healthMultiplier;

            body.baseRegen *= tracker.regenMultiplier;
            body.levelRegen *= tracker.regenMultiplier;

            body.baseMoveSpeed *= tracker.moveSpeedMultiplier;
            body.levelMoveSpeed *= tracker.moveSpeedMultiplier;

            body.baseAcceleration *= tracker.accelMultiplier;

            body.baseDamage *= tracker.damageMultiplier;
            body.levelDamage *= tracker.damageMultiplier;

            body.baseAttackSpeed *= tracker.attackSpeedMultiplier;
            body.levelAttackSpeed *= tracker.attackSpeedMultiplier;

            body.baseArmor *= tracker.armorMultiplier; 
            body.levelArmor *= tracker.armorMultiplier;

            body.transform.localScale *= tracker.sizeMultiplier;

            body.RecalculateStats();
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);
            //If body is dead, add to the dead list
            if(damageReport.victimBody == body)
            {
                GeneticMasterController.deadTrackers.Add(tracker);
                //Chat.AddMessage("Death of a " + body.baseNameToken + " with " + tracker.score.ToString() + " points.");
            }
        }

        private void CharacterBody_Update(On.RoR2.CharacterBody.orig_Update orig, CharacterBody self)
        {
            
            orig(self);
            //Every second its alive, give it a point
            if(self == body)
            {
                timePulse += Time.deltaTime;
                if (timePulse >= 1f)
                {
                    tracker.score += timePulse;
                    timePulse = 0f;
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            //Give tracker points equal to the damage it dealth
            if(damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>() == body)
            {
                tracker.score += damageInfo.damage;
            }
            else if (damageInfo.inflictor && damageInfo.inflictor.GetComponent<CharacterBody>() == body)
            {
                tracker.score += damageInfo.damage;
            }
        }
    }
}