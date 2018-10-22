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

    private BoidBehaviour boid;

    void Start ()
    {
        SeagullManager.Instance.Register(this);
        agent = GetComponent<NavMeshAgent>();
        boid = GetComponent<BoidBehaviour>();
        wanderCooldownCurrent = wanderCooldown;
        SetState(SeagullState.IDLE);
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (state == SeagullState.IDLE)
            {
                SetState(SeagullState.FLYING);
            }
            else
            {
                SetState(SeagullState.IDLE);
            }
            
        }

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
        boid.enabled = false;
        switch (newState)
        {
            case SeagullState.IDLE:
                transform.position = SeagullManager.Instance.eatPoint.position;
                agent.enabled = true;

                break;
            case SeagullState.WALKING:
                agent.enabled = true;
                break;
            case SeagullState.FLYING:
                transform.position = SeagullManager.Instance.flyPoint.position;
                agent.enabled = false;
                boid.enabled = true;
                break;
            default:
                break;
        }

        state = newState;
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
