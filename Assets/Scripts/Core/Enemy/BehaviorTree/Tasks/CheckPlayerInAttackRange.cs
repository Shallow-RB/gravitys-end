using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

namespace BehaviorTree.Tasks
{
    public class CheckPlayerInAttackRange : Node
    {
        private float _attackRange;
        private Transform _target;
        private Transform _transform;
        private EnemyRangeAttackController _enemyRangeAttackController;

        public CheckPlayerInAttackRange(float attackRange, Transform target, Transform transform, EnemyRangeAttackController enemyRangeAttackController)
        {
            _attackRange = attackRange;
            _target = target;
            _transform = transform;
            _enemyRangeAttackController = enemyRangeAttackController;
        }

        public override NodeState Evaluate()
        {
            float playerDistance = Vector3.Distance(_target.transform.position, _transform.position);

            if (playerDistance <= _attackRange)
            {
                _enemyRangeAttackController.rangeWeapon.allowRaycast = true;
                state = NodeState.SUCCESS;
            }
            else
            {
                _enemyRangeAttackController.rangeWeapon.allowRaycast = false;
                state = NodeState.FAILURE;
            }

            return state;
        }
    }
}
