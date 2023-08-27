using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers.Player
{
    public class StateMachine
    {
        public State currentState;

        public void Initialize(State startingState)
        {
            currentState = startingState;
            startingState.Enter();
        }

        public void ChangeState(State newState)
        {
            currentState.Exit();

            currentState = newState;
            newState.Enter();
        }


    }
}