using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Player
{
    public class State
    {
        public Character character;
        public StateMachine stateMachine;

        protected Vector3 velocity;
        protected Vector2 input;

        public InputAction moveAction;
        public InputAction lookAction;
        public InputAction attackAction;
        public InputAction pickupAction;



        public State(Character _character, StateMachine _stateMachine)
        {
            character = _character;
            stateMachine = _stateMachine;

            moveAction = character.playerInput.actions["Move"];
            lookAction = character.playerInput.actions["Look"];
            attackAction = character.playerInput.actions["Attack"];
            pickupAction = character.playerInput.actions["Loot Pickup"];
            
        }

        public virtual void Enter()
        {
            // Debug.Log("enter state: " + this.ToString());
        }

        public virtual void HandleInput()
        {
        }

        public virtual void LogicUpdate()
        {
        }

        public virtual void PhysicsUpdate()
        {
        }

        public virtual void Exit()
        {
        }
    }
}