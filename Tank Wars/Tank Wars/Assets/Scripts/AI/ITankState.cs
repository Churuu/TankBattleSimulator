using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITankState
{
    void OnEnterState();

    void UpdateState();

    void OnCollisionEnter(Collision other);

    void ToPatrolState();

    void ToAttackState();

    void ToChaseState();
}
