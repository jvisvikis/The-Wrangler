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

    private IEnumerator MoveBack(Vector2 dir, GameObject lasso)
    {
        while(state == State.BeingWrangled)
        {
            lasso.transform.position = (Vector2)lasso.transform.position + dir * Random.Range(0,pullStrength) * Time.deltaTime;
            yield return null;
        }
    }
}
