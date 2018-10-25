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
    private Food selectedFood = null;

    private float landProgress = 0f;
    private Vector3 landPointStart;
    private Quaternion landRotStart;

    private uint foodPoints = 0;

    void Start ()
    {
        SeagullManager.Instance.Register(this);
        agent = GetComponent<NavMeshAgent>();
        boid = GetComponent<BoidBehaviour>();
        wanderCooldownCurrent = wanderCooldown;
        SetState(SeagullState.FLYING);
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetState(SeagullState.FLYING);
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
                    if (Vector3.Distance(food.transform.position, transform.position) < foodAggroDistance && 
                        food.isPickedUp == false)
                    {
                        selectedFood = food;
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
                foreach (var food in Food.foodList)
                {
                    if (Vector3.Distance(food.transform.position, SeagullManager.Instance.landPoint.position) < foodAggroDistance && 
                        food.isPickedUp == false)
                    {
                        selectedFood = food;
                        SetState(SeagullState.LANDING);
                    }
                }
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
                agent.SetDestination(selectedFood.transform.position);
                if (Vector3.Distance(selectedFood.transform.position, transform.position) < .3f)
                {
                    SetState(SeagullState.EATING);
                }
                break;
            case SeagullState.EATING:
                bool finishedEating = this.animator.GetCurrentAnimatorStateInfo(0).IsName("peck") == false;
                if (finishedEating)
                {
                    selectedFood.Eat();
                    foodPoints++;

                    if (foodPoints >= 3)
                    {
                        //TODO: Set follower state
                    }
                    else
                    {
                        SetState(SeagullState.IDLE);
                    }
                }
                break;
            default:
                break;
        }
    }

    private void SetState(SeagullState newState)
    {
        boid.enabled = false;
        animator.SetBool("idle", false);
        animator.SetBool("walk", false);
        animator.SetBool("fly", false);
        animator.SetBool("landing", false);
        agent.enabled = false;
        switch (newState)
        {
            case SeagullState.IDLE:
                animator.SetBool("idle", true);
                transform.position = SeagullManager.Instance.landPoint.position;
                agent.enabled = true;
                break;
            case SeagullState.FLYING:
                animator.SetBool("fly", true);
                transform.position = SeagullManager.Instance.flyPoint.position;
                boid.enabled = true;
                break;
            case SeagullState.LANDING:
                animator.SetBool("landing", true);
                landPointStart = transform.position;
                landRotStart = transform.rotation;
                landProgress = 0;
                break;
            case SeagullState.WALKING_TO_FOOD:
                animator.SetBool("walk", true);
                transform.position = SeagullManager.Instance.landPoint.position;
                agent.enabled = true;
                break;
            case SeagullState.EATING:
                animator.Play("peck");
                agent.enabled = false;
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
