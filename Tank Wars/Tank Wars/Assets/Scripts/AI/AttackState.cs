using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class AttackState : ITankState
{

    private StatePatternTank parent;

    float timerDelta;

    public AttackState(StatePatternTank parent)
    {
        this.parent = parent;
    }

    public void OnEnterState()
    {
        parent.agent.isStopped = true;
    }

    public void UpdateState()
    {
        UpdateVisibleTanks();
        AttackEnemyTanks();
    }

    void AttackEnemyTanks()
    {
        if (parent.visibleTanks.Count < 1)
            return;

        Collider closestTank = GetClosestTank();

        StatePatternTank enemyTank = closestTank.GetComponent<StatePatternTank>();

        Vector3 dir = enemyTank.transform.position - parent.transform.position;

        float singleStep = 1.0f * Time.deltaTime;

        Vector3 newLookDirection = Vector3.RotateTowards(parent.turret.transform.forward, dir, singleStep, 0.0f);
        newLookDirection.y = 0;

        Debug.DrawRay(parent.turret.transform.position, newLookDirection * 100, Color.red);

        parent.turret.transform.rotation = Quaternion.LookRotation(newLookDirection);

        RaycastHit hit;
        Physics.Raycast(parent.turret.transform.position, newLookDirection, out hit);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Tank"))
            {
                Shoot(hit);
            }
        }
    }

    void Shoot(RaycastHit hit)
    {
        if (Time.time > timerDelta)
        {
            parent.PlaySound(parent.audioClips.shooting, parent.transform.position);
            parent.PlaySound(parent.audioClips.hit, parent.transform.position);
            parent.InstaniateObject(parent.explosionEffect, hit.point);
            hit.collider.GetComponent<StatePatternTank>().tankHealth--;
            timerDelta = Time.time + parent.shotCooldown;
        }
    }

    void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }


    Collider GetClosestTank()
    {
        Collider closestTank = parent.visibleTanks[0];
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < parent.visibleTanks.Count; i++)
        {
            float distance = Vector3.Distance(parent.transform.position, parent.visibleTanks[i].transform.position);
            if (distance < closestDistance && parent.visibleTanks[i] != parent.gameObject)
            {
                closestDistance = distance;
                closestTank = parent.visibleTanks[i];
            }
        }

        return closestTank;
    }

    void UpdateVisibleTanks()
    {
        if (parent.visibleTanks.Count < 1)
            ToPatrolState();

        //Quick fix for escape
        if (parent.tankHealth < parent.maxTankHealth / 2)
            ToPatrolState();
    }

    public void ToPatrolState()
    {
        parent.currentState = parent.patrolState;
        parent.currentState.OnEnterState();
    }

    public void ToAttackState()
    {
        Debug.LogError("Cannot transition to current state");
    }

    public void ToChaseState()
    {
        parent.currentState = parent.chaseState;
        parent.currentState.OnEnterState();
    }

    public void OnCollisionEnter(Collision other)
    {

    }
}
