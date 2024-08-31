using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    private State state = State.Idle;
    private enum State {Idle, Moving, Scared, BeingWrangled, Follow}
    public bool isShiny;
    public bool captured => state == State.Follow;
    public float moveNum = 10;
    public string animalName;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer animalSprite;
    [SerializeField] private Sprite animalFree;
    [SerializeField] private Sprite animalCaptured;
    [SerializeField] private float pullStrength;
    [SerializeField] private float wanderRadius;
    [SerializeField] private float maxIdleTime;
    [SerializeField] private float minIdleTime;
    [SerializeField] private float scaredSpeedModifier;
    [SerializeField] private float scaredDistance;
    [SerializeField] private AnimalType animalType;
    private Merchant merchant;
    private PlayerController player;
    private NavMeshAgent agent;
    private float idleTimer;
    private float waitTime;
    private float origSpeed;

    [Header("FMOD")]
    [SerializeField] private FMODUnity.StudioEventEmitter fmodAnimalScared;

    public float AnimalTypeValue => (float)animalType;

    void Start()
    {
        transform.eulerAngles = Vector3.zero;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        merchant = FindObjectOfType<Merchant>();
        player = FindObjectOfType<PlayerController>();
        origSpeed = agent.speed;
        EnterIdleState();
        SetSpriteDirection(false);
    }

    void Update()
    {
        animator.SetFloat("Speed",agent.velocity.magnitude/origSpeed);
        switch(state)
        {
            case State.Idle:
                if(Vector2.Distance((Vector2)transform.position,(Vector2)player.transform.position) < scaredDistance )
                {
                    EnterScaredState(player.transform);
                    break;
                }

                if(Vector2.Distance((Vector2)transform.position,(Vector2)merchant.transform.position) < scaredDistance)
                {
                    EnterScaredState(merchant.transform);
                    break;
                }

                if(idleTimer < waitTime)
                {
                    idleTimer += Time.deltaTime;
                }
                else
                {
                    EnterMovingState();
                }
                break;

            case State.Moving:
                if(Vector2.Distance((Vector2)transform.position,(Vector2)player.transform.position) < scaredDistance )
                {
                    EnterScaredState(player.transform);
                    break;
                }

                if(Vector2.Distance((Vector2)transform.position,(Vector2)merchant.transform.position) < scaredDistance)
                {
                    EnterScaredState(merchant.transform);
                    break;
                }
                
                if(Vector2.Distance((Vector2)agent.destination,(Vector2)transform.position)< 0.2f)
                {
                    EnterIdleState();
                }
                break;
            case State.BeingWrangled:
                agent.SetDestination(transform.position);
                break;
            case State.Scared:
                if(Vector2.Distance((Vector2)agent.destination,(Vector2)transform.position)< 0.2f)
                {
                    agent.speed = origSpeed;
                    EnterIdleState();
                }
                break;

        }
    }

    public void SetSpriteDirection(bool isLeft)
    {
        if(isLeft)
            animalSprite.flipX = false;
        else
            animalSprite.flipX = true;
    }

    public void SetSprite(bool isFree)
    {
        if(isFree)
            animalSprite.sprite = animalFree;
        else
            animalSprite.sprite = animalCaptured;
    }


    public void EnterIdleState()
    {
        SetSprite(true);
        idleTimer = 0;
        waitTime = Random.Range(minIdleTime, maxIdleTime);
        animator.SetBool("Walking",false);
        state = State.Idle;
    }

    public void EnterMovingState()
    {
        Vector2 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);
        SetSpriteDirection(newPos.x-transform.position.x < 0);
        animator.SetBool("Walking",true);
        state = State.Moving;
    }

    public void EnterScaredState(Transform scaredOf)
    {
        SetSprite(false);
        animator.SetTrigger("Scared");
        Vector2 newPos = RandomNavSphere(transform.position+(-10*(scaredOf.position-transform.position).normalized), 1, -1);
      
        agent.SetDestination(newPos);
        /*
            make speed of agent faster here
        */ 
        agent.speed = origSpeed*3;
        SetSpriteDirection(newPos.x-transform.position.x < 0);
        animator.SetBool("Walking",true);
        state = State.Scared;
    }

    public void EnterBeingWrangledState()
    {
        animator.SetBool("Walking",false);
        SetSprite(false);
        state = State.BeingWrangled;
    }

    public void EnterFollowState()
    {
        animator.SetBool("Walking",true);
        state = State.Follow;
    }

    public void Follow(Vector2 position)
    {
        if(state == State.Follow)
        {
            agent.SetDestination(position);
            if((Vector2)agent.destination == (Vector2)transform.position)
                animator.SetBool("Walking",false);
            else
                animator.SetBool("Walking",true);
        }
    }

    public void ExitFollowState()
    {
        EnterScaredState(player.transform);
    }

    public void PullBack(Vector2 dir, GameObject lasso)
    {
        state = State.BeingWrangled;
        StartCoroutine(MoveBack(dir, lasso));
    }

    public void Wrangled()
    {
        agent.speed = origSpeed*3;
        EnterFollowState();
    }

    public void Release(bool isFree)
    {
        if(isFree)
        {
            agent.enabled = true;
            EnterScaredState(player.transform);
        }
    }

    public void MoveTo(Vector2 target, float duration)
    {
        StartCoroutine(GoTo(target,duration));
    }

    private IEnumerator MoveBack(Vector2 dir, GameObject lasso)
    {
        while(state == State.BeingWrangled)
        {
            transform.position = (Vector2)transform.position + dir * Random.Range(0,pullStrength) * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator GoTo(Vector2 target, float duration)
    {
        float timer = 0;
        Vector2 startPos = (Vector2)transform.position;
        while(timer < duration)
        {
            transform.position = Vector2.Lerp(startPos, target, timer/duration);
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

     public Vector2 RandomNavSphere(Vector2 origin, float dist, int layermask) {
        Vector2 randDirection = Random.insideUnitSphere * dist;
 
        randDirection += origin;
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
 
        return navHit.position;
    }

    private IEnumerator StuckChecker()
    {
        Vector3 pos = transform.position;
        yield return new WaitForSeconds(0.5f);
        if(pos == transform.position)
            EnterIdleState();
    } 
}
