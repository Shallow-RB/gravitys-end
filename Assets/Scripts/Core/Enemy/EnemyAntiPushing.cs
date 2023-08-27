using Controllers;
using Controllers.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAntiPushing : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    private EnemyController controller;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        controller = transform.root.GetComponent<EnemyController>();
        navMeshAgent = transform.root.GetComponent<NavMeshAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !controller.isKnockbackInProgress)
        {
            rb.isKinematic = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !controller.isKnockbackInProgress)
        {
            rb.isKinematic = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (controller.playerNearby && other.gameObject.CompareTag("Enemy"))
        {
            // Checks distance between enemies
            var enemyDistance = Vector3.Distance(transform.root.position, other.transform.position);

            if (!(enemyDistance < controller.retreatDistance)) return;

            var direction = transform.position - other.transform.position;
            direction.y = 0f; // don't move up/down

            // Move enemies away from eachother so they don't collide
            if (navMeshAgent.isOnNavMesh)
                navMeshAgent.Move(direction.normalized * Time.deltaTime);
        }
    }
}
