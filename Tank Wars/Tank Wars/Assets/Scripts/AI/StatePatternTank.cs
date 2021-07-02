using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct AudioClips
{
    public AudioClip shooting, hit, moving, rotating;
}

public class StatePatternTank : MonoBehaviour
{
    public ITankState attackState, chaseState, patrolState, escapeState;
    protected ITankState currentState;

    public GameObject turret;
    public GameObject explosionEffect;

    public Vector2 patrolSize;
    public Vector3 respawnPoint;

    private bool isRotating;
    private AudioSource audioSource;

    public float tankSpeed;
    public float TurretRotationSpeed;
    public float visibilityRange;
    public float shotCooldown;
    public float patrolCooldownTime;

    public float tankDamage;
    public float tankHealth;
    public float maxTankHealth;

    public LayerMask tankLayer;

    public List<Collider> visibleTanks = new List<Collider>();

    public AudioClips audioClips;


    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public Vector3 previousTargetPosition;

    // Start is called before the first frame update
    void Start()
    {
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        escapeState = new EscapeState(this);
        agent = GetComponent<NavMeshAgent>();

        currentState = patrolState;
        currentState.OnEnterState();

        agent.speed = tankSpeed;
    }

    void Update()
    {
        currentState.UpdateState();
        UpdateNearbyTanks();
        OnDeath();
    }


    public void TryToMoveTank(Vector3 position)
    {
        Vector3 direction = position - agent.transform.position;
        direction.y = 0;
        float degreesToRotate = Vector3.Angle(direction, agent.transform.forward);
        Vector3 cross = Vector3.Cross(agent.transform.forward, direction);

        if (!isRotating)
        {
            agent.isStopped = true;
            isRotating = true;
            LeanTween.rotateY(gameObject, transform.eulerAngles.y + (cross.y > 0 ? degreesToRotate : -degreesToRotate), 1f).setOnComplete(() => MoveTank(position));
        }
    }

    void MoveTank(Vector3 position)
    {
        isRotating = false;
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    void OnDeath()
    {
        if(tankHealth <= 0)
        {
            tankHealth = maxTankHealth;
            Invoke("RespawnTank", 10);
            gameObject.SetActive(false);
        }
    }

    void RespawnTank()
    {
        gameObject.SetActive(true);
        transform.position = respawnPoint;
    }

    private void OnDrawGizmos()
    {
        Vector3 patrolArea = new Vector3(patrolSize.x, 0.5f, patrolSize.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, patrolArea);


        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visibilityRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(respawnPoint, 0.4f);
    }

    public GameObject InstaniateObject(GameObject obj, Vector3 pos)
    {
        GameObject instaniatedObject = Instantiate(obj, pos, transform.rotation);

        return instaniatedObject;
    }

    void UpdateNearbyTanks()
    {
        Collider[] visibleTanks = Physics.OverlapSphere(transform.position, visibilityRange, tankLayer);
        this.visibleTanks.Clear();
        foreach (var tank in visibleTanks)
        {
            if (tank.gameObject != gameObject && !this.visibleTanks.Contains(tank))
                this.visibleTanks.Add(tank);
        }
    }

    public void SwitchCurrentState(ITankState state)
    {
        currentState = state;
        currentState.OnEnterState();
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        GameObject clipObject = new GameObject("Sound");
        clipObject.transform.position = position;
        PlaySound soundComponent = clipObject.AddComponent<PlaySound>();
        soundComponent.PlayClip(clip);
    }

    public Collider GetClosestTank()
    {
        Collider closestTank = visibleTanks[0];
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < visibleTanks.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, visibleTanks[i].transform.position);
            if (distance < closestDistance && visibleTanks[i] != gameObject)
            {
                closestDistance = distance;
                closestTank = visibleTanks[i];
            }
        }

        return closestTank;
    }

    public Vector3 GetRandomPositionInsideBox(Vector3 center, Vector2 boxSize)
    {
        Vector3 randomPosition = new Vector3(
            (Random.value - 0.5f) * boxSize.x,
            0.0f,
            (Random.value - 0.5f) * boxSize.y
            );

        return center + randomPosition;
    }
}
