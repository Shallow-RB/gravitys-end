using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using BehaviorTree.Tasks;
using Controllers;
using UnityEngine;

public class MeleeEnemyBT : BTree
{
    private EnemyController enemyController;
    private EnemyMeleeAttackController enemyMeleeAttackController;

    protected override Node SetupTree()
    {
        enemyController = GetComponent<EnemyController>();
        enemyMeleeAttackController = GetComponent<EnemyMeleeAttackController>();

        Node root = new Selector(new List<Node>
        {   
            // Knockback sequence
            new Sequence(new List<Node>
            {
                // Check and do knockback
                new TaskKnockback(enemyController)
            }),
            // Attack player sequence
            new Sequence(new List<Node>
            {
                // Check if player is in attack range
                new CheckPlayerInMeleeAttackRange(enemyMeleeAttackController.attackRange, enemyController.target, enemyController.transform),
                // Perform melee attack
                new TaskMeleeAttack(enemyMeleeAttackController)
            }),
            // Follow player sequence
            new Sequence(new List<Node>
            {
                // Is Player in range
                new CheckPlayerInRange(enemyController.transform, enemyController.target, enemyController.lookRadius),
                // Follow the player
                new TaskMeleeFollow(enemyController.target, enemyController.agent, enemyController.transform)
            })
        });

        return root;
    }
}