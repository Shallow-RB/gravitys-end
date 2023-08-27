using UI.Inventory;
using UnityEngine;

namespace Controllers.Player
{
    public class SprintState : State
    {
        private float playerSpeed;
        private bool sprint;

        public SprintState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
        {
            character = _character;
            stateMachine = _stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            sprint = false;
            input = Vector2.zero;
            velocity = Vector3.zero;
            playerSpeed = character.playerSpeed;
        }

        public override void HandleInput()
        {
            base.Enter();

            input = moveAction.ReadValue<Vector2>();
            velocity = new Vector3(input.x, 0, input.y);

            //if there is no input, sprint is false
            sprint = moveAction.ReadValue<Vector2>() != Vector2.zero ? true : false;
        }

        public override void LogicUpdate()
        {
            if (sprint)
            {
                PlayerAnimator.Instance._animator.SetFloat("Velocity", input.magnitude + 0.15f, 0.1f, Time.deltaTime);
            }
            else
            {
                stateMachine.ChangeState(character.standing);
            }
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
    }
}
