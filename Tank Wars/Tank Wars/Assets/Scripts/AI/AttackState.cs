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
        UpdateStates();
        ShootVisibleEnemies();
    }

    void ShootVisibleEnemies()
    {
        if (parent.visibleTanks.Count < 1)
            return;

        Collider closestTank = parent.GetClosestTank();

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
                parent.previousTargetPosition = hit.collider.transform.position;
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
            hit.collider.GetComponent<StatePatternTank>().tankHealth -= parent.tankDamage;
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

    void UpdateStates()
    {
        if (parent.visibleTanks.Count < 1)
            ToChaseState();

        //Quick fix for escape
        if (parent.tankHealth < parent.maxTankHealth / 2)
            ToEscapeState();
    }

    public void ToPatrolState()
    {
        parent.SwitchCurrentState(parent.patrolState);
    }

    public void ToAttackState()
    {
        Debug.LogError("Cannot transition to current state");
    }

    public void ToChaseState()
    {
        parent.SwitchCurrentState(parent.chaseState);
    }

    public void OnCollisionEnter(Collision other)
    {

    }

    public void ToEscapeState()
    {
        parent.SwitchCurrentState(parent.escapeState);
    }
}
