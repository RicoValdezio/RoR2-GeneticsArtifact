using RoR2;
using UnityEngine;

namespace GeneticsArtifact
{
    public class MasterGeneBehaviour : MonoBehaviour
    {
        public GeneTracker tracker;
        public CharacterMaster master;
        public float timePulse, timeAlive = 1f, damageDealt = 0f;
        public bool isMutable = false;

        private void OnEnable()
        {
            master = gameObject.GetComponent<CharacterMaster>();
            if (master.GetBody() is CharacterBody body)
            {
                tracker = new GeneTracker(body.bodyIndex);

                //If the body should mutate, do that; otherwise just apply the existing mutation
                if ((body.teamComponent.teamIndex == TeamIndex.Monster) ||
                    (body.teamComponent.teamIndex == TeamIndex.Neutral && ConfigMaster.applyToNeutrals.Value) ||
                    (body.teamComponent.teamIndex == TeamIndex.Player && ConfigMaster.applyToMinions.Value && !body.master.playerCharacterMasterController))
                {
                    MutateFromParent();
                    isMutable = true;
                }
                ApplyMutation();

                //Log the spawn values if the logging flags are enabled
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
        }

        private void OnDisable()
        {
            tracker.Dispose();
            Destroy(this);
        }

        public void MutateFromParent()
        {
            tracker.MutateFromParent();
        }

        public void RapidMutate()
        {
            if (isMutable)
            {
                tracker.MutateSelf();
            }
        }

        public void ApplyMutation()
        {
            if (master.GetBody() is CharacterBody body)
            {
                //If Size is enabled, apply it
                if (tracker.GetGeneValue("Size") is float sizeMultiplier)
                {
                    body.transform.localScale *= sizeMultiplier;
                }

                body.RecalculateStats();
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
