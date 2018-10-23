using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Seagull : MonoBehaviour {

    enum SeagullState
    {
        IDLE,
        WALKING,
        FLYING,
        APPROACHING_FOOD
    }

    [Header("Walking")]
    public float wanderRadius = 1f;
    public float wanderCooldown = 1f;

    [SerializeField] private SeagullState state;
    [SerializeField] private Animator animator;
    [SerializeField] private float foodAggroDistance = 10f;

    //Walking
    private Transform target;
    private NavMeshAgent agent;
    private float wanderCooldownCurrent;

    private BoidBehaviour boid;
    private Food approachingFood = null;

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
                foreach (var food in Food.foodList)
                {
                    if (Vector3.Distance(food.transform.position, transform.position) < foodAggroDistance)
                    {
                        approachingFood = food;
                        SetState(SeagullState.APPROACHING_FOOD);
                    }
                }

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
            case SeagullState.APPROACHING_FOOD:
                agent.SetDestination(approachingFood.transform.position);
                break;
            default:
                break;
        }
    }

    private void SetState(SeagullState newState)
    {
        boid.enabled = false;
        animator.SetBool("walk", false);
        animator.SetBool("fly", false);
        switch (newState)
        {
            case SeagullState.IDLE:
                animator.SetBool("walk", true);
                transform.position = SeagullManager.Instance.eatPoint.position;
                agent.enabled = true;
                
                break;
            case SeagullState.WALKING:
                animator.SetBool("walk", true);
                agent.enabled = true;
                break;
            case SeagullState.FLYING:
                animator.SetBool("fly", true);
                transform.position = SeagullManager.Instance.flyPoint.position;
                agent.enabled = false;
                boid.enabled = true;
                break;
            case SeagullState.APPROACHING_FOOD:
                //TODO: Split into approaching flying/walking?
                animator.SetBool("walk", true);
                transform.position = SeagullManager.Instance.eatPoint.position;
                agent.enabled = true;
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
