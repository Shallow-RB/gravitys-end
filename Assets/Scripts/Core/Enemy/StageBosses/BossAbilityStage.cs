using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Enemy.StageBosses
{
    [Serializable]
    public class BossAbilityStage
    {
        [SerializeField]
        private int healthStageActivation;

        [SerializeField]
        private List<BossAbilitySequence> bossAbilitySequences;

        public List<BossAbilitySequence> GetBossAbilitySequences()
        {
            return bossAbilitySequences;
        }

        public int GetHealhStageActivation()
        {
            return healthStageActivation;
        }
    }
}
