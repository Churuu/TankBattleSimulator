using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This state is intended for the tank to flank other tanks
 * 
 * */

public class ChaseState : ITankState
{

    private StatePatternTank parent;

    public ChaseState(StatePatternTank parent)
    {
        this.parent = parent;
    }

    public void OnEnterState()
    {

    }

    public void UpdateState()
    {

    }

    public void ToPatrolState()
    {
        parent.currentState = parent.patrolState;
        parent.currentState.OnEnterState();
    }

    public void ToAttackState()
    {
        parent.currentState = parent.attackState;
        parent.currentState.OnEnterState();
    }

    public void ToChaseState()
    {
        Debug.LogError("Cannot transition to current state");
    }


    public void OnCollisionEnter(Collision other)
    {

    }
}
