using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using Controllers;
using UnityEngine;

namespace BehaviorTree.Tasks
{
    public class TaskKnockback : Node
    {
        private EnemyController _enemyController;

        public TaskKnockback(EnemyController enemyController)
        {
            _enemyController = enemyController;
        }

        public override NodeState Evaluate()
        {
            if (_enemyController.isKnockbackInProgress)
            {
                //Ryan: play knockback animation here
                state = NodeState.RUNNING;
            }
            else
            {
                state = NodeState.FAILURE;
            }
            
            return state;
        }
    }
}
