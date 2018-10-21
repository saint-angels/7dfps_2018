using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Seagull : MonoBehaviour {

    enum SeagullState
    {
        IDLE,
        WALKING,
        FLYING
    }

    [Header("Walking")]
    public float wanderRadius = 1f;
    public float wanderCooldown = 1f;

    [SerializeField] private SeagullState state;

    //Walking
    private Transform target;
    private NavMeshAgent agent;
    private float wanderCooldownCurrent;

    void Start ()
    {
        SeagullManager.Instance.Register(this);
        agent = GetComponent<NavMeshAgent>();
        wanderCooldownCurrent = wanderCooldown;
    }

    void Update ()
    {
        switch (state)
        {
            case SeagullState.IDLE:
                wanderCooldownCurrent += Time.deltaTime;
                if (wanderCooldownCurrent >= wanderCooldown)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                    agent.SetDestination(newPos);
                    wanderCooldownCurrent = 0;
                }
                break;
            case SeagullState.WALKING:
                break;
            case SeagullState.FLYING:
                break;
            default:
                break;
        }
    }

    private void SetState(SeagullState newState)
    {
        switch (newState)
        {
            case SeagullState.IDLE:
                break;
            case SeagullState.WALKING:
                break;
            case SeagullState.FLYING:
                break;
            default:
                break;
        }
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}
