using RoR2;
using UnityEngine;

namespace GeneticsArtifact.GeneticControllers
{
    public class PlayerGeneBehaviour : MonoBehaviour
    {
        public GeneTracker tracker;
        public CharacterMaster master;

        private void OnEnable()
        {
            master = gameObject.GetComponent<CharacterMaster>();
            if(master.GetBody() is CharacterBody body)
            {
                tracker = new GeneTracker(body.bodyIndex, true);
            }
        }

        private void OnDisable()
        {
            tracker.Dispose();
            Destroy(this);
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

                if (ConfigMaster.spawnLogging.Value)
                {
                    GeneticsArtifactPlugin.geneticLogSource.LogInfo(tracker.BuildGenePairMessage());
                }
            }
        }
    }
}
