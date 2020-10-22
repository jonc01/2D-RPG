using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    public State currentState { get; private set; } //can get any class, private only sets in here

    public void Initialize(State startingState)
    {
        currentState = startingState;
        currentState.Enter(); //
    }

    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
