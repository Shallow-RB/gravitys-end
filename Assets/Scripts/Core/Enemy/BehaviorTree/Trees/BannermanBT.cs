using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using BehaviorTree.Tasks;
using Controllers;
using UnityEngine;

public class BannermanBT : BTree
{
    private EnemyController enemyController;
    private BannermanController bannermanController;

    protected override Node SetupTree()
    {
        enemyController = GetComponent<EnemyController>();
        bannermanController = GetComponent<BannermanController>();

        Node root = new Selector(new List<Node>
        {   
            // Knockback sequence
            new Sequence(new List<Node>
            {
                // Check and do knockback
                new TaskKnockback(enemyController)
            }),

            // While following, also heal
            new Parallel(new List<Node>
            {
                // Follow player sequence
                new Sequence(new List<Node>
                {
                    // Is Player in range
                    new CheckPlayerInRange(enemyController.transform, enemyController.target, enemyController.lookRadius),
                    // Follow the player
                    new TaskFollow(enemyController.target, enemyController.agent, enemyController)
                }),
                // Healing sequence
                new Sequence(new List<Node>
                {
                    // Is Player in range
                    new CheckPlayerInRange(enemyController.transform, enemyController.target, enemyController.lookRadius),
                    // Healing
                    new TaskHeal(enemyController.transform, enemyController.target, enemyController.lookRadius, bannermanController)
                })
            })
        });

        return root;
    }
}