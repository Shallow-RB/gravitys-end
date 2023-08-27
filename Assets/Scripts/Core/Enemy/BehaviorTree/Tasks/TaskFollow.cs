using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;
using Controllers;

namespace BehaviorTree.Tasks
{
    public class TaskFollow : Node
    {
        private Transform _target;
        private NavMeshAgent _agent;
        private EnemyController _enemyController;

        public TaskFollow(Transform target, NavMeshAgent agent, EnemyController enemyController)
        {
            _target = target;
            _agent = agent;
            _enemyController = enemyController;
        }

        public override NodeState Evaluate()
        {
            bool destinationValid = _agent.SetDestination(_target.position);

            if (destinationValid)
            {
                if (_enemyController.enemyAnimator.runtimeAnimatorController.name == "BannermanAnim")
                {
                    _enemyController.enemyAnimator.SetTrigger("walk");
                }

                if (_enemyController.enemyAnimator.runtimeAnimatorController.name == "commonRangedAnim")
                {
                    _enemyController.enemyAnimator.SetBool("stand_shoot", false);
                    _enemyController.enemyAnimator.SetBool("run_shoot", true);
                }
                state = NodeState.RUNNING;
            }
            else
                state = NodeState.FAILURE;

            return state;
        }
    }
}
