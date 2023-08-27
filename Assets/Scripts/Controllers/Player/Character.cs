using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Controllers.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(InputManager))]
    public class Character : MonoBehaviour
    {
        [Header("Controls")]
        public float playerSpeed = 5.0f;

        [HideInInspector]
        public CharacterController controller;

        [HideInInspector]
        public PlayerInput playerInput;

        [HideInInspector]
        public Camera _camera;

        [HideInInspector]
        public Vector3 lookAtPosition;

        [HideInInspector]
        public Animator animator;

        [HideInInspector]
        public Vector3 playerVelocity;

        [HideInInspector]
        public int attackCount;

        public StateMachine movementSM;
        public SprintState sprinting;
        public StandingState standing;
        public CombatState combatting;
        public AttackState attacking;

        private GameObject _player;
        private bool _gamePaused;
        private List<WallSeeThrough> currentWalls = new();

        public LayerMask allowedLayers;

        [Header("Effects")]
        public ParticleSystem hitParticle;


        // Start is called before the first frame update
        private void Start()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            playerInput = FindObjectOfType<PlayerInput>();
            _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

            movementSM = new StateMachine();
            standing = new StandingState(this, movementSM);
            sprinting = new SprintState(this, movementSM);
            combatting = new CombatState(this, movementSM);
            attacking = new AttackState(this, movementSM);

            movementSM.Initialize(standing);

            _player = PlayerManager.Instance.player;
            UIHandler.OnPauseGameToggle += OnPauseGameToggle;
        }

        private void Update()
        {
            if (_gamePaused || DialogueManager.instance.dialogueActive || Navigation.instance.loadingScreenActive)
            {
                PlayerAnimator.Instance._animator.SetFloat("Velocity", 0, 0.1f, Time.deltaTime);
                movementSM.ChangeState(standing);
                PlayerAnimator.Instance._animator.SetTrigger("move");
            }
            else
            {
                movementSM.currentState.HandleInput();
                movementSM.currentState.LogicUpdate();
            }
        }

        private void OnPauseGameToggle(bool gamePaused)
        {
            _gamePaused = gamePaused;
        }

        private void FixedUpdate()
        {
            movementSM.currentState.PhysicsUpdate();
        }

        private void CheckWall(WallSeeThrough wall)
        {
            if (!currentWalls.Contains(wall))
            {
                currentWalls.Add(wall);
                wall.GetComponent<WallSeeThrough>().DisableTiles();
            }
        }

        private void RemoveWalls(List<WallSeeThrough> filteredWalls)
        {
            if (currentWalls.Count > 0)
            {
                currentWalls.Except(filteredWalls).ToList().ForEach(wall =>
                {
                    wall.EnableTiles();
                    currentWalls.Remove(wall);
                });
            }
        }
    }
}
