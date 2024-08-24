using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    private State state = State.Idle;
    private enum State {Idle, Moving, BeingWrangled, Follow}

    public float moveNum = 10;
    public string animalName;
    [SerializeField] private float pullStrength;

    public void PullBack(Vector2 dir, GameObject lasso)
    {
        state = State.BeingWrangled;
        StartCoroutine(MoveBack(dir, lasso));
    }

    public void Wrangled()
    {
        state = State.Follow;
    }

    public void Release()
    {
        state = State.Idle;
    }

    public void MoveTo(Vector2 target, float duration)
    {
        StartCoroutine(GoTo(target,duration));
    }

    private IEnumerator MoveBack(Vector2 dir, GameObject lasso)
    {
        while(state == State.BeingWrangled)
        {
            lasso.transform.position = (Vector2)lasso.transform.position + dir * Random.Range(0,pullStrength) * Time.deltaTime;
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
}
