using UnityEngine;

namespace Controllers.Player
{
    public class StandingState : State
    {
        private float playerSpeed;
        private bool sprint;

        public StandingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
        {
            character = _character;
            stateMachine = _stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            sprint = false;
            input = Vector2.zero;

            velocity = character.playerVelocity;
            playerSpeed = character.playerSpeed;
            PlayerAnimator.Instance._animator.SetFloat("Velocity", 0, 0.1f, Time.deltaTime);
        }

        public override void HandleInput()
        {
            base.HandleInput();

            if (moveAction.triggered) sprint = true;

            input = moveAction.ReadValue<Vector2>();
            velocity = new Vector3(input.x, 0, input.y);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            PlayerAnimator.Instance._animator.SetFloat("Velocity", 0, 0.1f, Time.deltaTime);
            if (PlayerAnimator.Instance._animator.GetFloat("Velocity") < 0.001)
            {
                PlayerAnimator.Instance._animator.SetFloat("Velocity", 0);
            }

            if (EquipmentSystem.Instance._equippedWeapon != null)
            {
                stateMachine.ChangeState(character.combatting);
            }

            if (sprint) stateMachine.ChangeState(character.sprinting);

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            velocity = Quaternion.Euler(0, -45, 0) * velocity;

            character.controller.Move(velocity * Time.deltaTime * playerSpeed);

            if (velocity.sqrMagnitude > 0)
            {
                character.transform.rotation = Quaternion.Slerp(character.transform.rotation,
                    Quaternion.LookRotation(velocity), 0.2f);
            }
        }

        public override void Exit()
        {
            base.Exit();

            character.playerVelocity = new Vector3(input.x, 0, input.y);

            if (velocity.sqrMagnitude > 0) character.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
