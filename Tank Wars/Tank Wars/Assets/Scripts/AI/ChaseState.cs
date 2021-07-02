using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This state is intended for the tank to chase tanks escaping
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
        parent.TryToMoveTank(parent.previousTargetPosition);
    }

    public void UpdateState()
    {
        if (parent.agent.remainingDistance < 5.0f || parent.visibleTanks.Count > 0)
        {
            if (parent.visibleTanks.Count > 0)
                ToAttackState();
            else
                ToPatrolState();
        }
    }

    public void ToPatrolState()
    {
        parent.SwitchCurrentState(parent.patrolState);
    }

    public void ToAttackState()
    {
        parent.SwitchCurrentState(parent.attackState);
    }

    public void ToChaseState()
    {
        Debug.LogError("Cannot transition to current state");
    }

    public void ToEscapeState()
    {
        parent.SwitchCurrentState(parent.escapeState);
    }

    public void OnCollisionEnter(Collision other)
    {

    }

}
