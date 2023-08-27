using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using Controllers;
using UnityEngine;

namespace BehaviorTree.Tasks
{
    public class CheckPlayerInRange : Node
    {
        private Transform _transform;
        private Transform _target;
        private float _lookRadius;

        public CheckPlayerInRange(Transform transform, Transform target, float lookRadius)
        {
            _transform = transform;
            _target = target;
            _lookRadius = lookRadius;
        }

        public override NodeState Evaluate()
        {
            var targetPosition = _target.position;
            var transformPosition = _transform.position;
            var playerDistance = Vector3.Distance(targetPosition, transformPosition);

            if (playerDistance > _lookRadius)
            {
                state = NodeState.FAILURE;
            }
            else
            {
                state = NodeState.SUCCESS;
            }

            return state;
        }
    }
}
