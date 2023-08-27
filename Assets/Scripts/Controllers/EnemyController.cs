using System.Collections;
using Controllers.Enemy;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;
using Controllers.Player;
using Unity.VisualScripting;

namespace Controllers
{
    public class EnemyController : MonoBehaviour
    {
        public float lookRadius = 10f;
        public float retreatDistance = 2f;
        public float rotationSpeed = 5f;
        public float knockbackForce = 3f;
        public float knockbackDuration = .5f;
        public LayerMask obstacleMask;
        public Animator enemyAnimator;

        [Header("Effects")]
        public ParticleSystem hitParticle;

        [HideInInspector]
        public NavMeshAgent agent;

        [HideInInspector]
        public Transform target;

        [HideInInspector]
        public bool isKnockbackInProgress = false;
        [HideInInspector]
        public Vector3 knockbackDirection;

        private BTree behaviorTree;

        private Rigidbody rb;

        [HideInInspector]
        public bool playerNearby;

        private void Start()
        {
            behaviorTree = GetComponent<BTree>();

            rb = GetComponent<Rigidbody>();

            enemyAnimator = GetComponentInChildren<Animator>();

            // See PlayerManager.cs for explanation
            target = PlayerManager.Instance.player.transform;
            agent = GetComponent<NavMeshAgent>();

            var o = gameObject;
            Physics.IgnoreLayerCollision(o.layer, o.layer);

            behaviorTree.SetTree();
        }

        private void Update()
        {
            var distance = (target.position - transform.position).sqrMagnitude;

            if (distance > lookRadius * lookRadius)
            {
                behaviorTree.state = false;
                playerNearby = false;
                return;
            }

            var enemyDirection = target.position - transform.position;

            behaviorTree.state = true;
            playerNearby = true;
            
            // Check if there is no wall in between the player and the enemy, if there is then return
            if (Physics.Raycast(transform.position, enemyDirection.normalized, out var hit, distance,
                    LayerMask.GetMask("Entity"), QueryTriggerInteraction.Ignore))
            {
                if (!hit.collider.CompareTag("Enemy"))
                    return;
            }

            // Face the player
            FaceTarget();
        }

        // When the player is too close to the enemy, it wont rotate anymore
        // This function fixes it
        private void FaceTarget()
        {
            var direction = (target.position - transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            // Use Quaternion.Slerp instead of lookRotation to smooth out the animation
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        // Draws a sphere around the enemy to visualize the range of where the enemy will start chasing you
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, lookRadius / 2);
        }

        public IEnumerator PerformKnockback()
        {
            isKnockbackInProgress = true;

            // Disable kinematic to allow external forces to affect the enemy
            rb.isKinematic = false;

            // Apply the knockback force to the enemy's Rigidbody
            rb.velocity = target.forward * knockbackForce;

            yield return new WaitForSeconds(knockbackDuration);

            // Enable kinematic to stop external forces from affecting the enemy
            rb.isKinematic = true;

            isKnockbackInProgress = false;
        }
    }
}
