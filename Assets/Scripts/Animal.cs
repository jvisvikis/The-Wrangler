using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    private State state = State.Idle;
    private enum State {Idle, Moving, BeingWrangled, Follow}
    public bool isShiny;
    public bool captured => state == State.Follow;
    public float moveNum = 10;
    public string animalName;
    [SerializeField] private GameObject animalSprite;
    [SerializeField] private float pullStrength;
    [SerializeField] private float wanderRadius;
    [SerializeField] private float maxIdleTime;
    [SerializeField] private float minIdleTime;
    private NavMeshAgent agent;
    private Vector3 originalScale;
    private float idleTimer;
    private float waitTime;

    void Start()
    {
        transform.eulerAngles = Vector3.zero;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        originalScale = animalSprite.transform.localScale;
        EnterIdleState();
    }

    void Update()
    {
        switch(state)
        {
            case State.Idle:
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
                if((Vector2)agent.destination == (Vector2)transform.position)
                {
                    EnterIdleState();
                }
                break;
            case State.BeingWrangled:
                agent.SetDestination(transform.position);
                break;

        }
    }

    public void SetSpriteDirection(bool isLeft)
    {
        if(isLeft)
            animalSprite.transform.localScale = new Vector3(-originalSpriteScale.x,originalSpriteScale.y,originalSpriteScale.z);
        else
            animalSprite.transform.localScale = originalSpriteScale;
    }

    public void EnterIdleState()
    {
        idleTimer = 0;
        waitTime = Random.Range(minIdleTime, maxIdleTime);
        state = State.Idle;
    }

    public void EnterMovingState()
    {
        Vector2 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);
        SetSpriteDirection(newPos.x-transform.position.x < 0);
        state = State.Moving;
    }

    public void EnterBeingWrangledState()
    {
        state = State.BeingWrangled;
    }

    public void EnterFollowState()
    {
        state = State.Follow;
    }

    public void Follow(Vector2 position)
    {
        if(state == State.Follow)
        {
            agent.SetDestination(position);
        }
    }

    public void ExitFollowState()
    {
        EnterIdleState();
    }

    public void PullBack(Vector2 dir, GameObject lasso)
    {
        state = State.BeingWrangled;
        StartCoroutine(MoveBack(dir, lasso));
    }

    public void Wrangled()
    {
        EnterFollowState();
    }

    public void Release(bool isFree)
    {
        if(isFree)
        {
            agent.enabled = true;
            EnterIdleState();
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
}
