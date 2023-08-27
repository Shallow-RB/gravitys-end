using UnityEngine;

namespace Controllers.Player
{
    public class CombatState : State
    {
        private float playerSpeed;
        private bool attack;
        private bool sprint;

        public CombatState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
        {
            character = _character;
            stateMachine = _stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            attack = false;
            input = Vector2.zero;

            velocity = character.playerVelocity;
            playerSpeed = character.playerSpeed;
            PlayerAnimator.Instance._animator.SetFloat("Velocity", 0);
        }

        public override void HandleInput()
        {
            base.HandleInput();

            if (attackAction.triggered) attack = true;

            input = moveAction.ReadValue<Vector2>();
            velocity = new Vector3(input.x, 0, input.y);

            //if there is no input, sprint is false
            sprint = moveAction.ReadValue<Vector2>() != Vector2.zero ? true : false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (EquipmentSystem.Instance._equippedWeapon == null)
            {
                stateMachine.ChangeState(character.standing);
            }

            if (sprint)
            {
                PlayerAnimator.Instance._animator.SetFloat("Velocity", input.magnitude + 0.15f, 0.1f, Time.deltaTime);
            }
            else
            {
                PlayerAnimator.Instance._animator.SetFloat("Velocity", 0, 0.1f, Time.deltaTime);
                if (PlayerAnimator.Instance._animator.GetFloat("Velocity") < 0.001)
                {
                    PlayerAnimator.Instance._animator.SetFloat("Velocity", 0);
                }
            }

            if (attack)
            {
                if (EquipmentSystem.Instance._equippedWeapon.GetComponent<MeleeWeapon>())
                {
                    stateMachine.ChangeState(character.attacking);
                }


                if (EquipmentSystem.Instance._equippedWeapon.CompareTag("Ranged"))
                {
                    if (EquipmentSystem.Instance._equippedWeapon.GetComponent<RangeWeapon>().CanShoot())
                    {
                        stateMachine.ChangeState(character.attacking);
                    }
                    // if (!EquipmentSystem.Instance._equippedWeapon.GetComponent<RangeWeapon>().CanShoot() && PlayerAnimator.Instance._animator.velocity.magnitude < 0.1f)
                    // {
                    //     PlayerAnimator.Instance._animator.SetTrigger("run_reload");
                    //     stateMachine.ChangeState(character.combatting);
                    // }

                    stateMachine.ChangeState(character.combatting);
                }
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

        public override void Exit()
        {
            base.Exit();

            character.playerVelocity = new Vector3(input.x, 0, input.y);

            if (velocity.sqrMagnitude > 0) character.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}


