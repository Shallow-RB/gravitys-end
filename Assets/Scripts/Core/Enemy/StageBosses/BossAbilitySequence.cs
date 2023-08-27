using System;
using System.Collections;
using System.Collections.Generic;
using Core.Enemy.StageBosses;
using UnityEngine;

[Serializable]
public class BossAbilitySequence
{
        [SerializeField]
        private BossAbility bossAbility;

        [SerializeField]
        private int amountOfTimes;

        private int _amountOfTimesUsed;

        public BossAbility GetBossAbility()
        {
            return bossAbility;
        }

        public int GetAmountOfTimes()
        {
            return amountOfTimes;
        }

        public int GetAmountOfTimesUsed()
        {
            return _amountOfTimesUsed;
        }

        public void SetAmountOfTimesUsed(int used)
        {
            _amountOfTimesUsed = used;
        }

        public void IncrementAmountOfTimesUsed()
        {
            _amountOfTimesUsed++;
        }
}
