using System.Collections;
using System.Linq;
using Core.Enemy;
using Core.StageGeneration.Rooms.RoomTypes;
using UnityEngine;
using UnityEngine.AI;

namespace Controllers.Enemy
{
    public class BossController : MonoBehaviour
    {
        [SerializeField]
        private float rotationSpeed;

        [SerializeField]
        private Material hitMaterial;

        private NavMeshAgent _agent;
        private Boss _boss;
        private BossRoom _bossRoom;
        private Material _originalMaterial;
        private Renderer _renderer;
        private Transform _target;
        private Animator _bossAnimator;
        private bool _allowMovement;

        private void Start()
        {
            _target = PlayerManager.Instance.player.transform;
            _agent = BossManager.Instance.boss.GetComponent<NavMeshAgent>();
            _boss = BossManager.Instance.boss.GetComponent<Boss>();
            _bossRoom = transform.root.gameObject.GetComponent<BossRoom>();
            _renderer = BossManager.Instance.boss.GetComponentInChildren<Renderer>();
            _bossAnimator = BossManager.Instance.boss.GetComponentInChildren<Animator>();

            _originalMaterial = _renderer.material;
        }

        private void Update()
        {
            if (!_boss.GetStartFight()) return;

            var target = _target.position;
            var distance = Vector3.Distance(target, transform.position);

            if (distance > _agent.stoppingDistance)
            {
                WalkToPlayer(target);
                _bossAnimator.SetFloat("velocity", _agent.velocity.magnitude);

            }
            else
            {
                _bossAnimator.SetFloat("velocity", 0);

                FaceTarget();
            }
        }

        private void FixedUpdate()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);


            Collider player = colliders.Where(c => c.CompareTag("Player")).FirstOrDefault();

            if (player != null)
            {
                _allowMovement = false;
            }
            else
            {
                _allowMovement = true;
            }
        }

        private void FaceTarget()
        {
            var direction = (_target.position - transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        private void WalkToPlayer(Vector3 target)
        {
            if (_allowMovement)
            {
                _agent.SetDestination(target);
            }
        }
    }
}
