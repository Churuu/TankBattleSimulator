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
        parent.currentState = parent.attackState;
        parent.currentState.OnEnterState();
    }

    public void ToChaseState()
    {
        parent.currentState = parent.chaseState;
        parent.currentState.OnEnterState();
    }

    public void OnCollisionEnter(Collision other)
    {

    }

    void Patrol()
    {
        if (Time.time > timerDelta && parent.agent.remainingDistance < 1.0f)
        {
            Vector3 newPatrolPosition = GetRandomPositionInsideBox(Vector3.zero, parent.patrolSize);

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

    Vector3 GetRandomPositionInsideBox(Vector3 center, Vector2 boxSize)
    {
        Vector3 randomPosition = new Vector3(
            (Random.value - 0.5f) * boxSize.x,
            0.0f,
            (Random.value - 0.5f) * boxSize.y
            );

        return center + randomPosition;
    }
}
