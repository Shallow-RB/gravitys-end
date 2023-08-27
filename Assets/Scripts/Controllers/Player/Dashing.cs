using System;
using System.Collections;
using Core.Audio;
using UI;
using UnityEngine;
using Utils;

namespace Controllers.Player
{
    public class Dashing : MonoBehaviour
    {
        [Header("Dash")]
        [SerializeField]
        private float dashSpeed = 20f;

        [SerializeField]
        [Tooltip("Dash duration in seconds")]
        private float dashDuration = 0.25f;

        [SerializeField]
        [Tooltip("Dash countdown timer when starting dash")]
        private float dashTimer;

        [SerializeField]
        [Tooltip("Time in seconds for dash to be available again")]
        private float dashCooldown;

        [SerializeField]
        private bool dashAvailable = true;

        [SerializeField]
        private bool isDashing;

        [Header("Trail")]
        public float activeTime = 1f;
        private bool isTrailActive;

        [Header("Mesh related")]
        public float meshRefreshRate = 0.1f;
        public float meshDestroyDelay = 0.2f;
        private SkinnedMeshRenderer[] skinnedMeshRenderers;
        public Transform positionToSpawn;

        [Header("Shader related")]
        public Material material;
        public string shaderVarRef;
        public float shaderVarRate = 0.1f;
        public float shaderVarResfreshRate = 0.05f;

        private CharacterController _controller;
        private bool _dashInput;
        private GameInput _gameInput;
        private Vector2 _movementInput;
        private bool _gamePaused;

        // Start is called before the first frame update
        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _gameInput = FindObjectOfType<GameInput>();
            UIHandler.OnPauseGameToggle += OnPauseGameToggle;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isDashing) HandleInput();
        }

        private void FixedUpdate()
        {
            if (_gamePaused || isDashing || DialogueManager.instance.dialogueActive || Navigation.instance.loadingScreenActive)
                return;

            HandleDash();
        }

        private void OnPauseGameToggle(bool gamePaused)
        {
            _gamePaused = gamePaused;
        }

        private void HandleInput()
        {
            Character character = PlayerManager.Instance.player.GetComponent<Character>();
            if (character.movementSM.currentState == character.standing || character.movementSM.currentState == character.attacking)
            {
                return;
            }

            _movementInput = _gameInput.GetMovement();
            if (_movementInput == Vector2.zero)
            {
                return;
            }

            _dashInput = _gameInput.GetDash();
        }



        //////////////////////////////////////////////
        // Dashing                                  // 
        //////////////////////////////////////////////

        private void HandleDash()
        {
            switch (dashAvailable)
            {
                case false when _dashInput:
                    //text on screen to tell player dash is on cooldown
                    break;
                case true when _dashInput:
                    //when activation input is pressed, start dash cooldown
                    StartCoroutine(DashCoroutine());
                    if (!isTrailActive)
                    {
                        isTrailActive = true;
                        StartCoroutine(ActivateTrail(activeTime));
                    }
                    break;
            }
        }

        private IEnumerator DashCoroutine()
        {
            // disable user input
            isDashing = true;
            dashAvailable = false;
            // set dash timer
            dashTimer = dashDuration;
            var dashDir = new Vector3(_movementInput.x, 0, _movementInput.y);
            //makes player move independent of camera rotation (W means north, S means south, etc.)
            dashDir = Quaternion.Euler(0, -45, 0) * dashDir;
            SoundEffectsManager.instance.PlaySoundEffect(SoundEffect.DASH);

            while (dashTimer > 0)
            {
                // Move player
                _controller.Move(dashDir * (dashSpeed * Time.deltaTime));

                // Decrease dash timer
                dashTimer -= Time.deltaTime;

                yield return null;
            }

            // Enable user input after the dash
            isDashing = false;

            // Wait for dash cooldown
            yield return new WaitForSeconds(dashCooldown);

            // Re-enable dash availability
            dashAvailable = true;
        }

        public float GetDashTimer()
        {
            return dashTimer;
        }

        public bool GetDashAvailable()
        {
            return dashAvailable;
        }

        public void SetDashAvailable(bool available)
        {
            dashAvailable = available;
        }



        private IEnumerator ActivateTrail(float timeActive)
        {
            while (timeActive > 0)
            {
                timeActive -= meshRefreshRate;

                if (skinnedMeshRenderers == null)
                {
                    skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                }

                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                {
                    GameObject gObj = new GameObject();
                    gObj.transform.position = positionToSpawn.position;
                    gObj.transform.rotation = positionToSpawn.rotation;


                    MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                    MeshFilter mf = gObj.AddComponent<MeshFilter>();

                    Mesh mesh = new Mesh();
                    skinnedMeshRenderers[i].BakeMesh(mesh);

                    mf.mesh = mesh;
                    mr.material = material;

                    StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarResfreshRate));
                    Destroy(gObj, meshDestroyDelay);
                }

                yield return new WaitForSeconds(meshRefreshRate);
            }
            isTrailActive = false;
        }

        private IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshrate)
        {
            float valueToAnimate = mat.GetFloat(shaderVarRef);
            while (valueToAnimate > goal)
            {
                valueToAnimate -= rate;
                mat.SetFloat(shaderVarRef, valueToAnimate);
                yield return new WaitForSeconds(refreshrate);
            }
        }
    }
}