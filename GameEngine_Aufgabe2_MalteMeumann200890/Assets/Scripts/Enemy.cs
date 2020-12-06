using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform[] destinations;
    private int currentDestination=0;
    private bool start=false;
    private float agentvelocity;
    private Animator animator;
    private EnemyState currentState;
    [Range(0,5)]
    public float attackRange;

    public float lifePoints;
    
    private Transform currentTarget;
    public float dealDamage;
    public float hitfrequency;
    private float punshstart;
    private Vector3 currentTargetPosition = Vector3.zero;
    [SerializeField]
    private GameObject ui;
    private GUIHandler uiHandler;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentDestination = 0;
        currentState = EnemyState.Patrouille;
        uiHandler = ui.GetComponent<GUIHandler>();

    }

    // Update is called once per frame
    void Update()
    {


        switch (currentState)
        {
            default:
            case EnemyState.Patrouille:
                handlePatroullie();
                break;
            case EnemyState.Verfolgen:
                handleVerfolgen();
                break;
            case EnemyState.Attack:
               hitCurrentTarget();
                break;
            case EnemyState.Dead:
                handleDeadSequence();
                break;
        }


       




    }

    private void handleDeadSequence()
    {
        if (!agent.isStopped)
        {
            agent.isStopped = true;
        }
        else
        {
            this.GetComponent<MeshCollider>().enabled = false;
            animator.SetBool("isDying", true);
           
            Invoke("destroyObject", 3f);

        }
    }

    public void getHit(float hitPoints)
    {
        lifePoints -= hitPoints;
        if (lifePoints <= 0 && currentState!=EnemyState.Dead)
        {
            uiHandler.increaseScore();
            currentState = EnemyState.Dead;
        }
       
    }
    private void destroyObject()
    {
        Destroy(this.gameObject);
    }
    private void handlePatroullie()
    {
        if (agent.stoppingDistance != 1)
        {
            agent.stoppingDistance = 1;
        }

        if (!(agent.remainingDistance > agent.stoppingDistance))
        {
            Debug.Log("stopped");
            if (currentDestination == destinations.Length - 1)
            {
                currentDestination = 0;
            }
            else
            {
                currentDestination = currentDestination += 1;
            }
            agent.SetDestination(destinations[currentDestination].position);

        }

        if (start == false )
        {
            currentDestination++;
            agent.SetDestination(destinations[currentDestination].position);
            start = true;
        }

        agentvelocity = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", agentvelocity);
    }


    private void handleVerfolgen()
    {
        if (agent.stoppingDistance != 4)
        {
            agent.stoppingDistance = 4;
        }
        if (!(agent.remainingDistance > agent.stoppingDistance+attackRange))
        {
            //hit enemy
            Debug.Log("Reached Enemy");
            agent.isStopped = true;
            //reachedEnemy = true;
            //hitCurrentTarget();

            currentState = EnemyState.Attack;

        }

        agentvelocity = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", agentvelocity);
    }

    public void enemyIsSeen(Transform[] targets)
    {
        float distanceToClosestTarget = 100;
        int indexClosestTarget = 0;
        // check wich Target in Angle is the closest
        for (int i = 0; i < targets.Length; i++)
        {
            float distanceToTarget = (targets[i].position - transform.position).magnitude;
            if (distanceToTarget < distanceToClosestTarget)
            {
                distanceToClosestTarget = distanceToTarget;
                indexClosestTarget = i;
            }
        }
        if (currentTargetPosition != targets[indexClosestTarget].position)
        {
            currentTargetPosition = targets[indexClosestTarget].position;

            
                currentTarget = targets[indexClosestTarget];
                currentTarget.GetComponent<testTarget>().subscribeEnemy(this.gameObject);
           

            agent.SetDestination(currentTargetPosition);
            currentState = EnemyState.Verfolgen;
        }
       
    }

    public void resetTarget()
    {
        
      // Debug.Log(this.name + " reset");
        if (currentTarget !=null) {
            if (currentTarget.GetComponent<testTarget>().desubscribeEnemy(this.gameObject))
            {
                Debug.Log(this.name + " Desubcribed");
                animator.SetBool("isPunshing", false);
                punshstart = 0;
                currentTarget = null;
                agent.isStopped = false;
            }
        }
        
       //agent.SetDestination(destinations[currentDestination].position);
       currentState = EnemyState.Patrouille;
        
    }

    private void hitCurrentTarget()
    {
        animator.SetBool("isPunshing", true);
        if (punshstart != 0)
        {
            if (Time.time >= punshstart + hitfrequency)
            {
                
                punshstart = Time.time;
                currentTarget.GetComponent<testTarget>().hitTarget(dealDamage);
            }
        }
        else
        {
            punshstart = Time.time;
            currentTarget.GetComponent<testTarget>().hitTarget(dealDamage);
        }
    }

    public void targetIsDead()
    {
        Debug.Log("Target is Dead ");
        animator.SetBool("isPunshing", false);
        punshstart = 0;
        //reachedEnemy = false;
        //destinationIsTarget = false;
        agent.SetDestination(destinations[currentDestination].position);
        currentState = EnemyState.Patrouille;
    }

    public void inform(TargetState state)
    {
        switch (state)
        {
            default:
            case TargetState.TargetDead:
                targetIsDead();
                break;
            case TargetState.TargetLeave:
                break;
            case TargetState.TargetEnter:
                break;
        }
    }


   public void setCurrentState(EnemyState state)
    {
        currentState = state;
    }

    
    public EnemyState getCurrentState()
    {
        return this.currentState;
    }


    /*
     Debug.Log(this.name + " Enemy is seen and " + targets.Length);

         int indexClosestTarget = 0;
         float distanceLastTarget = 100;

         if (targets.Length != 0) { 
             for (int i = 0; i < targets.Length; i++)
             {
                 float distanceToTarget = (targets[i].position - transform.position).magnitude;
                 if (distanceToTarget < distanceLastTarget)
                 {
                 distanceLastTarget = distanceToTarget;
                 indexClosestTarget = i;
                 }
             }

             if (currentTargetPosition != targets[indexClosestTarget].position)
             {
                 currentTargetPosition = targets[indexClosestTarget].position;
                 if (!destinationIsTarget)
                 {
                 destinationIsTarget = true;
                 }
                 currentTarget = targets[indexClosestTarget];
                 currentTarget.GetComponent<testTarget>().subscribeEnemy(this.gameObject);
                 agent.stoppingDistance = 2f;
                // agent.SetDestination(currentTargetPosition);
             }
     */



    /* if (!destinationIsTarget)
     {
         if (!(agent.remainingDistance > agent.stoppingDistance))
         {
             Debug.Log("stopped");
             if (currentDestination == destinations.Length - 1)
             {
                 currentDestination = 0;
             }
             else
             {
                 currentDestination = currentDestination += 1;
             }
             agent.SetDestination(destinations[currentDestination].position);

         }

         if (start == false)
         {
             currentDestination++;
             agent.SetDestination(destinations[currentDestination].position);
             start = true;
         }

     }
     else
     {
         //Destination is a Target
         if (!(agent.remainingDistance > agent.stoppingDistance))
         {
             //hit enemy
             Debug.Log("Reached Enemy");
             reachedEnemy = true;
             hitCurrentTarget();

         }
     }

     agentvelocity = agent.velocity.magnitude / agent.speed;
     animator.SetFloat("Speed", agentvelocity);
    */
}
