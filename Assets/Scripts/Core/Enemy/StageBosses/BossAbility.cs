using System.Collections;
using UnityEngine;

namespace Core.Enemy.StageBosses
{
    public abstract class BossAbility : MonoBehaviour
    {
        public abstract IEnumerator UseBossAbility();
    }
}
