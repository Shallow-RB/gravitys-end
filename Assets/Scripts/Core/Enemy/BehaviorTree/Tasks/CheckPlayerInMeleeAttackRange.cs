using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

namespace BehaviorTree.Tasks
{
    public class CheckPlayerInMeleeAttackRange : Node
    {
        private float _attackRange;
        private Transform _target;
        private Transform _transform;

        public CheckPlayerInMeleeAttackRange(float attackRange, Transform target, Transform transform)
        {
            _attackRange = attackRange;
            _target = target;
            _transform = transform;
        }

        public override NodeState Evaluate()
        {
            float playerDistance = Vector3.Distance(_target.transform.position, _transform.position);

            if (playerDistance <= _attackRange)
            {
                state = NodeState.SUCCESS;
            }
            else
            {
                state = NodeState.FAILURE;
            }

            return state;
        }
    }
}
