using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using Controllers;
using UnityEngine;

namespace BehaviorTree.Tasks
{
    public class TaskHeal : Node
    {
        private Transform _transform;
        private Transform _target;
        private float _lookRadius;
        private BannermanController _bannermanController;

        public TaskHeal(Transform transform, Transform target, float lookRadius, BannermanController bannermanController)
        {
            _transform = transform;
            _target = target;
            _lookRadius = lookRadius;
            _bannermanController = bannermanController;
        }

        public override NodeState Evaluate()
        {
            var targetPosition = _target.position;
            var transformPosition = _transform.position;
            var playerDistance = Vector3.Distance(targetPosition, transformPosition);

            if (playerDistance > _lookRadius)
            {
                _bannermanController.healingAllowed = false;
                state = NodeState.FAILURE;
            }
            else
            {
                _bannermanController.healingAllowed = true;
                state = NodeState.RUNNING;
            }

            return state;
        }
    }
}
