using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree.Tasks
{
    public class TaskRetreat : Node
    {
        private Transform _transform;
        private Transform _target;
        private float _retreatDistance;
        private LayerMask _obstacleMask;
        private NavMeshAgent _agent;

        public TaskRetreat(Transform transform, Transform target, float retreatDistance, LayerMask obstacleMask, NavMeshAgent agent)
        {
            _transform = transform;
            _target = target;
            _retreatDistance = retreatDistance;
            _obstacleMask = obstacleMask;
            _agent = agent;
        }

        public override NodeState Evaluate()
        {
            var targetPosition = _target.position;
            var transformPosition = _transform.position;
            var playerDistance = Vector3.Distance(targetPosition, transformPosition);

            if (playerDistance <= _retreatDistance)
            {
                Vector3 retreatDirection = _transform.position - _target.position;
                Vector3 retreatDestination = _transform.position + retreatDirection.normalized * _retreatDistance;

                // Perform a raycast to check if the retreat destination is obstructed
                RaycastHit hit;
                if (Physics.Raycast(retreatDestination, -retreatDirection.normalized, out hit, _retreatDistance, _obstacleMask))
                {
                    // Debug.DrawRay(retreatDestination, -retreatDirection.normalized, Color.cyan);
                    // If there's an obstacle, find an alternate point nearby that is not obstructed
                    Vector3 newDestination = FindAlternateDestination(retreatDestination, retreatDirection.normalized);

                    if (newDestination != Vector3.zero)
                    {
                        retreatDestination = newDestination;
                    }
                    else
                    {
                        state = NodeState.FAILURE;
                        return state;
                    }
                }
                // Set the new retreat destination for the enemy
                _agent.updateRotation = false;
                _agent.SetDestination(retreatDestination);
                state = NodeState.RUNNING;
            }
            else
            {
                state = NodeState.FAILURE;
            }

            return state;
        }

        private Vector3 FindAlternateDestination(Vector3 originalDestination, Vector3 retreatDirection)
        {
            // Parameters for casting rays around the original destination
            int numRays = 8; // Number of rays to cast
            float angleStep = 45f; // Angle step between rays in degrees
            float rayDistance = 1f; // Distance to cast the rays

            // Cast rays in a circle around the original destination
            for (int i = 0; i < numRays; i++)
            {
                float angle = i * angleStep;
                Vector3 rayDirection = Quaternion.Euler(0f, angle, 0f) * retreatDirection;

                // Check if the ray hits an obstacle
                if (!Physics.Raycast(originalDestination, rayDirection, rayDistance, _obstacleMask))
                {
                    // Return the first unobstructed point found as the new destination
                    return originalDestination + rayDirection * rayDistance;
                }
            }

            // Unable to find an alternate destination
            return Vector3.zero;
        }
    }
}