using RoR2;
using UnityEngine;

namespace GeneticsArtifact
{
    internal class GeneBehaviour : MonoBehaviour
    {
        internal GeneTracker tracker;

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
        }
    }
}