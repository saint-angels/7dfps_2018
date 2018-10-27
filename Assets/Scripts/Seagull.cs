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
        EATING,
        WALKING_TO_TAKEOFF,
        TAKEOFF,
        FOLLOWING
    }

    [Header("Walking")]
    public float wanderRadius = 1f;
    public float wanderCooldown = 1f;

    public bool mySeagull = false;

    [SerializeField] private SeagullState state;
    [SerializeField] private Animator animator;
    [SerializeField] private float foodAggroDistance = 10f;

    //Walking
    private Transform target;
    private NavMeshAgent agent;
    private float wanderCooldownCurrent;

    private Food selectedFood = null;

    private float moveProgress = 0f;
    private Vector3 movePointStart;
    private Vector3 movePointFinish;
    private Quaternion moveRotationStart;
    private float flySpeed = 1f;

    private uint foodPoints = 0;
    private int[] growth = new int[] { 3, 6, 9, 12 };


    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        wanderCooldownCurrent = wanderCooldown;
        SetState(SeagullState.FLYING);
        RecalculateScale();
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

                if (mySeagull)
                {
                    //Check for landing reasons
                    foreach (var food in Food.foodList)
                    {
                        if (Vector3.Distance(food.transform.position, SeagullManager.Instance.landPoint.position) < foodAggroDistance && 
                            food.isPickedUp == false)
                        {
                            selectedFood = food;
                            SetState(SeagullState.LANDING);
                        }
                    }
                }

                transform.position = Vector3.Slerp(movePointStart, movePointFinish, moveProgress);
                //transform.rotation = Quaternion.Slerp(moveRotationStart, Quaternion.identity, moveProgress);

                Vector3 direction = movePointFinish - movePointStart;
                Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, moveProgress);

                moveProgress += flySpeed * Time.deltaTime;
                if (moveProgress >= 1f)
                {
                    SetState(SeagullState.FLYING);
                }

                break;
            case SeagullState.LANDING:
                transform.position = Vector3.Slerp(movePointStart, SeagullManager.Instance.landPoint.position, moveProgress);
                transform.rotation = Quaternion.Lerp(moveRotationStart, Quaternion.identity, moveProgress);
                moveProgress += flySpeed * Time.deltaTime;
                if (moveProgress >= 1f)
                {
                    SetState(SeagullState.IDLE);
                }
                break;
            case SeagullState.TAKEOFF:
                transform.position = Vector3.Slerp(movePointStart, SeagullManager.Instance.GetRandomFlyPoint(), moveProgress);
                transform.rotation = Quaternion.Lerp(moveRotationStart, Quaternion.identity, moveProgress);
                moveProgress += flySpeed * Time.deltaTime;
                if (moveProgress >= 1f)
                {
                    SetState(SeagullState.FLYING);
                }
                break;
            case SeagullState.WALKING_TO_FOOD:
                agent.SetDestination(selectedFood.transform.position);
                if (Vector3.Distance(selectedFood.transform.position, transform.position) < .5f)
                {
                    SetState(SeagullState.EATING);
                }
                break;
            case SeagullState.WALKING_TO_TAKEOFF:
                agent.SetDestination(SeagullManager.Instance.landPoint.position);
                if (Vector3.Distance(SeagullManager.Instance.landPoint.position, transform.position) < .3f)
                {
                    SetState(SeagullState.TAKEOFF);
                }
                break;
            case SeagullState.EATING:
                bool finishedEating = this.animator.GetCurrentAnimatorStateInfo(0).IsName("peck") == false;
                if (finishedEating)
                {
                    selectedFood.Eat();

                    foodPoints++;
                    RecalculateScale();

                    if (foodPoints >= 3)
                    {
                        SetState(SeagullState.FOLLOWING);
                    }
                    else
                    {
                        SetState(SeagullState.WALKING_TO_TAKEOFF);
                    }
                }
                break;
            case SeagullState.FOLLOWING:
                var playerT = Player.Instance.transform;
                agent.SetDestination(playerT.position + playerT.forward);
                break;
        }
    }

    private void SetState(SeagullState newState)
    {
        animator.SetBool("idle", false);
        animator.SetBool("walk", false);
        animator.SetBool("fly", false);
        animator.SetBool("landing", false);
        animator.SetBool("takeoff", false);
        agent.enabled = false;
        moveProgress = 0;
        flySpeed = Random.Range(.1f, 1f);
        switch (newState)
        {
            case SeagullState.IDLE:
                animator.SetBool("idle", true);
                agent.enabled = true;
                break;
            case SeagullState.FLYING:
                animator.SetBool("fly", true);
                moveRotationStart = transform.rotation;
                movePointStart = transform.position;
                movePointFinish = SeagullManager.Instance.GetRandomFlyPoint();
                break;
            case SeagullState.LANDING:
                animator.SetBool("landing", true);
                movePointStart = transform.position;
                moveRotationStart = transform.rotation;
                moveProgress = 0;
                break;
            case SeagullState.TAKEOFF:
                animator.SetBool("takeoff", true);
                movePointStart = transform.position;
                moveRotationStart = transform.rotation;
                moveProgress = 0;
                break;
            case SeagullState.WALKING_TO_FOOD:
                animator.SetBool("walk", true);
                transform.position = SeagullManager.Instance.landPoint.position;
                agent.enabled = true;
                break;
            case SeagullState.WALKING_TO_TAKEOFF:
                animator.SetBool("walk", true);
                agent.enabled = true;
                break;
            case SeagullState.EATING:
                animator.Play("peck");
                agent.enabled = false;
                break;
            case SeagullState.FOLLOWING:
                animator.SetBool("walk", true);
                agent.enabled = true;
                break;
            default:
                break;
        }

        state = newState;
    }

    private void RecalculateScale()
    {
        var newScale = growth[foodPoints];
        transform.localScale = new Vector3(newScale, newScale, newScale);
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
