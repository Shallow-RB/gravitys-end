using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using Controllers;
using UnityEngine;

namespace BehaviorTree.Tasks
{
    public class TaskShoot : Node
    {
        private EnemyRangeAttackController _enemyRangeAttackController;
        private Transform _transform;
        private EnemyController enemyController;

        public TaskShoot(EnemyRangeAttackController enemyRangeAttackController, Transform transform)
        {
            _enemyRangeAttackController = enemyRangeAttackController;
            _transform = transform;
            enemyController = _transform.GetComponent<EnemyController>();
        }

        public override NodeState Evaluate()
        {
            string animControllerName = enemyController.enemyAnimator.runtimeAnimatorController.name;
            EnemyRangeWeapon rangeWeapon = _enemyRangeAttackController.rangeWeapon;

            if (rangeWeapon.allowShot)
            {
                rangeWeapon.allowRaycast = true;
            }

            if (animControllerName == "BannermanAnim")
            {
                enemyController.enemyAnimator.SetTrigger("idle");
            }

            if (animControllerName == "commonRangedAnim")
            {
                enemyController.enemyAnimator.SetBool("run_shoot", false);
                enemyController.enemyAnimator.SetBool("stand_shoot", true);
            }
            state = NodeState.RUNNING;
            return state;
        }
    }
}
