using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;
using Controllers;

namespace BehaviorTree.Tasks
{
    public class TaskMeleeFollow : Node
    {
        private Transform _target;
        private NavMeshAgent _agent;
        private EnemyController _enemyController;

        public TaskMeleeFollow(Transform target, NavMeshAgent agent, Transform enemyTransform)
        {
            _target = target;
            _agent = agent;
            _enemyController = enemyTransform.GetComponent<EnemyController>();
        }

        public override NodeState Evaluate()
        {
            bool destinationValid = _agent.SetDestination(_target.position);
            _enemyController.enemyAnimator.SetBool("attack1", false);

            if (destinationValid)
            {
                _enemyController.enemyAnimator.SetTrigger("run");
                state = NodeState.RUNNING;
            }
            else
                state = NodeState.FAILURE;

            return state;
        }
    }
}
