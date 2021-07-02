using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : ITankState
{

    private StatePatternTank parent;

    float timerDelta;


    public PatrolState(StatePatternTank parent)
    {
        this.parent = parent;
    }

    public void OnEnterState()
    {
        parent.agent.isStopped = false;
    }

    public void UpdateState()
    {
        Patrol();
        GetVisibleTanks();
    }

    public void ToPatrolState()
    {
        Debug.LogError("Cannot transition to current state");
    }

    public void ToAttackState()
    {
        parent.SwitchCurrentState(parent.attackState);
    }

    public void ToChaseState()
    {
        parent.SwitchCurrentState(parent.chaseState);
    }

    public void ToEscapeState()
    {
        parent.SwitchCurrentState(parent.escapeState);
    }

    public void OnCollisionEnter(Collision other)
    {

    }

    void Patrol()
    {
        if (Time.time > timerDelta && parent.agent.remainingDistance < 1.0f)
        {
            Vector3 newPatrolPosition = parent.GetRandomPositionInsideBox(Vector3.zero, parent.patrolSize);

            parent.TryToMoveTank(newPatrolPosition);
            
            //Passive healing? Maybe do it another way?
            parent.tankHealth++;

            timerDelta = Time.time + parent.patrolCooldownTime;
        }
    }

    void GetVisibleTanks()
    {
        if(parent.visibleTanks.Count > 0 && parent.tankHealth > parent.maxTankHealth / 2)
        {
            ToAttackState();
        }
    }
}
