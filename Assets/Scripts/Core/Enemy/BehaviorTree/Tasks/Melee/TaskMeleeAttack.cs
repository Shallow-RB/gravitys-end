using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.Tasks
{
    public class TaskMeleeAttack : Node
    {
        private EnemyMeleeAttackController _enemyMeleeAttackController;

        public TaskMeleeAttack(EnemyMeleeAttackController enemyMeleeAttackController)
        {
            _enemyMeleeAttackController = enemyMeleeAttackController;
        }

        public override NodeState Evaluate()
        {
            _enemyMeleeAttackController.PerformMeleeAttack();
            state = NodeState.RUNNING; 
            return state;
        }
    }
}
