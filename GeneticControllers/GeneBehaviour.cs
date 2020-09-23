using RoR2;
using UnityEngine;

namespace GeneticsArtifact
{
    internal class GeneBehaviour : MonoBehaviour
    {
        internal GeneTracker tracker;
        internal CharacterBody body;

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
            }
        }
    }
}