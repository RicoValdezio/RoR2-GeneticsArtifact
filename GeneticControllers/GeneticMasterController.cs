using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticsArtifact
{
    internal class GeneticMasterController
    {
        internal static List<GeneTracker> masterTrackers;
        internal static List<GeneTracker> deadTrackers;
        internal static int maxTrackers = 1;
        internal static bool trackerPerMonsterID = true;

        internal static float timeBetweenUpdates = 10f, updateTimer = 0f;

        internal static void Init()
        {
            masterTrackers = new List<GeneTracker>();
            deadTrackers = new List<GeneTracker>();

            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.CharacterMaster.SpawnBody += CharacterMaster_SpawnBody;
            On.RoR2.Run.Update += Run_Update;
        }

        private static void BuildMasters()
        {
            if (trackerPerMonsterID)
            {
                //Do nothing, we'll add trackers when the first of a monster spawns
            }
            else
            {
                int currentIndex = 0;
                while (masterTrackers.Count < maxTrackers)
                {
                    masterTrackers.Add(new GeneTracker(currentIndex));
                    currentIndex++;
                }
            }
        }

        private static void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);
            //If the artifact is enabled
            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def.artifactIndex))
            {
                //If its the first stage in a run, reset all masters and purge all dead
                if (self.stageClearCount == 0)
                {
                    masterTrackers.Clear();
                    deadTrackers.Clear();
                    BuildMasters();
                }
            }
        }

        private static CharacterBody CharacterMaster_SpawnBody(On.RoR2.CharacterMaster.orig_SpawnBody orig, CharacterMaster self, GameObject bodyPrefab, Vector3 position, Quaternion rotation)
        {
            CharacterBody body = orig(self, bodyPrefab, position, rotation);
            //If the artifact is enabled and the body is a monster
            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def.artifactIndex) && body.teamComponent.teamIndex == TeamIndex.Monster)
            {
                //If using a master per monster type and there isn't already a master for this type, add a master for this type
                if (trackerPerMonsterID && masterTrackers.Find(x => x.index == body.bodyIndex) == null)
                {
                    masterTrackers.Add(new GeneTracker(body.bodyIndex, true));
                    //Chat.AddMessage("A new Master was made for bodyIndex: " + body.baseNameToken);
                }
                //Always add a behaviour to the body
                body.gameObject.AddComponent<GeneBehaviour>();
            }
            return body;
        }

        private static void Run_Update(On.RoR2.Run.orig_Update orig, Run self)
        {
            orig(self);
            if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.def.artifactIndex))
            {
                updateTimer += Time.deltaTime;
                if (updateTimer >= timeBetweenUpdates)
                {
                    updateTimer = 0f;
                    //Clean up the deaths, will process these later
                    deadTrackers.Clear();
                }
            }
        }
    }
}