using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Seagull : MonoBehaviour {

    enum SeagullState
    {
        IDLE,
        FLYING,
        WALKING_TO_FOOD,
        LANDING,
        EATING
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

    private float landProgress = 0f;
    private Vector3 landPointStart;
    private Quaternion landRotStart;

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetState(SeagullState.FLYING);
            //if (state == SeagullState.IDLE)
            //{
                
            //}
            //else
            //{
            //    SetState(SeagullState.IDLE);
            //}
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetState(SeagullState.LANDING);
        }

        switch (state)
        {
            case SeagullState.IDLE:
                foreach (var food in Food.foodList)
                {
                    if (Vector3.Distance(food.transform.position, transform.position) < foodAggroDistance)
                    {
                        approachingFood = food;
                        SetState(SeagullState.WALKING_TO_FOOD);
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
            case SeagullState.FLYING:
                break;
            case SeagullState.LANDING:
                transform.position = Vector3.Slerp(landPointStart, SeagullManager.Instance.landPoint.position, landProgress);
                transform.rotation = Quaternion.Lerp(landRotStart, Quaternion.identity, landProgress);
                landProgress += .5f * Time.deltaTime;
                if (landProgress >= 1f)
                {
                    SetState(SeagullState.IDLE);
                }
                print(landProgress);
                break;
            case SeagullState.WALKING_TO_FOOD:
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
        animator.SetBool("landing", false);
        switch (newState)
        {
            case SeagullState.IDLE:
                animator.SetBool("walk", true);
                transform.position = SeagullManager.Instance.landPoint.position;
                agent.enabled = true;
                break;
            case SeagullState.FLYING:
                animator.SetBool("fly", true);
                transform.position = SeagullManager.Instance.flyPoint.position;
                agent.enabled = false;
                boid.enabled = true;
                break;
            case SeagullState.LANDING:
                animator.SetBool("landing", true);
                landPointStart = transform.position;
                landRotStart = transform.rotation;
                landProgress = 0;
                break;
            case SeagullState.WALKING_TO_FOOD:
                //TODO: Split into approaching flying/walking?
                animator.SetBool("walk", true);
                transform.position = SeagullManager.Instance.landPoint.position;
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
