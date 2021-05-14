using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticsArtifact
{
    public class MasterGeneBehaviour
    {
        public BodyIndex bodyIndex;
        public Dictionary<GeneStat, float> templateGenes;

        public void Init()
        {
            templateGenes = new Dictionary<GeneStat, float>();
            foreach (GeneStat stat in Enum.GetValues(typeof(GeneStat)))
            {
                templateGenes.Add(stat, 1f);
            }
        }
    }
}
