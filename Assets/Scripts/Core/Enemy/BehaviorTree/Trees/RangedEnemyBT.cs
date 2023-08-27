using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using BehaviorTree.Tasks;
using Controllers;
using UnityEngine;

public class RangedEnemyBT : BTree
{
    private EnemyController enemyController;
    private EnemyRangeAttackController enemyRangeAttackController;

    protected override Node SetupTree()
    {
        enemyController = GetComponent<EnemyController>();
        enemyRangeAttackController = GetComponent<EnemyRangeAttackController>();

        Node root = new Selector(new List<Node>
        {
            // Knockback sequence
            new Sequence(new List<Node>
            {
                // Check and do knockback
                new TaskKnockback(enemyController)
            }),
            // Retreat parallel
            new Parallel(new List<Node>
            {
                // While retreating from the player also shoot
                new Sequence(new List<Node>
                {
                    // Is player in range
                    new CheckPlayerInRange(enemyController.transform, enemyController.target, enemyController.lookRadius),
                    // Retreat from player
                    new TaskRetreat(enemyController.transform, enemyController.target, enemyController.retreatDistance, enemyController.obstacleMask, enemyController.agent),
                }),
                new Sequence(new List<Node>
                {
                    // Is player in attack range
                    new CheckPlayerInAttackRange(enemyRangeAttackController.attackRange, enemyController.target, enemyController.transform, enemyRangeAttackController),
                    // Shoot at player
                    new TaskShoot(enemyRangeAttackController, enemyController.transform)
                })
            }),
            // Follow player parrallel
            new Parallel(new List<Node>
            {
                // While following the player shoot also
                new Sequence(new List<Node>
                {
                    // Is Player in range
                    new CheckPlayerInRange(enemyController.transform, enemyController.target, enemyController.lookRadius),
                    // Follow the player
                    new TaskFollow(enemyController.target, enemyController.agent, enemyController),
                }),
                new Sequence(new List<Node>
                {
                    // Is player in attack range
                    new CheckPlayerInAttackRange(enemyRangeAttackController.attackRange, enemyController.target, enemyController.transform, enemyRangeAttackController),
                    // Shoot at player
                    new TaskShoot(enemyRangeAttackController, enemyController.transform)
                })
            }),
        });

        return root;
    }
}
