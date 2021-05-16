using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void MutateFromChildren()
        {
            List<MonsterGeneBehaviour> children = GeneEngineDriver.deadGenes.Where(x => x.bodyIndex == bodyIndex && x.score > 0).ToList();
            foreach (GeneStat stat in Enum.GetValues(typeof(GeneStat)))
            {
                float totalScore = 0f;
                float totalValue = 0f;
                foreach (MonsterGeneBehaviour child in children)
                {
                    totalValue += child.currentGenes[stat] * child.score;
                    totalScore += child.score;
                }
                if(totalScore > 0) templateGenes[stat] = totalValue / totalScore;
            }
        }
    }
}
