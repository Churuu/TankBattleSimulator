using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeState : ITankState
{

    private StatePatternTank parent;

    float timerDelta;


    public EscapeState(StatePatternTank parent)
    {
        this.parent = parent;
    }

    public void OnCollisionEnter(Collision other)
    {
        throw new System.NotImplementedException();
    }

    public void OnEnterState()
    {

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
        Debug.LogError("Cannot enter same state as current state");
    }

    public void ToPatrolState()
    {
        parent.SwitchCurrentState(parent.patrolState);
    }

    public void UpdateState()
    {
        EscapeAndHide();
        ShootVisibleEnemies();
        UpdateStates();
    }

    private void UpdateStates()
    {
        if (parent.visibleTanks.Count < 1)
            ToPatrolState();
    }

    void EscapeAndHide() // Cheat way of escaping
    {

        if (Time.time > timerDelta && parent.agent.remainingDistance < 1.0f)
        {
            Vector3 newPatrolPosition = parent.GetRandomPositionInsideBox(Vector3.zero, parent.patrolSize);

            float distanceToEnemy = Vector3.Distance(newPatrolPosition, parent.previousTargetPosition);
            while (distanceToEnemy < 10.0f)
            {
                newPatrolPosition = parent.GetRandomPositionInsideBox(Vector3.zero, parent.patrolSize);
                distanceToEnemy = Vector3.Distance(newPatrolPosition, parent.previousTargetPosition);

                Debug.Log("Chosen escape position was too close to tank");
            }

            parent.TryToMoveTank(newPatrolPosition);

            parent.tankHealth++;

            timerDelta = Time.time + parent.patrolCooldownTime;
        }
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
}
