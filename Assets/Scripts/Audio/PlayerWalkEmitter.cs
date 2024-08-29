using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Hack until there is an Animation that the footsteps can hook into
/// </summary>
public class PlayerWalkEmitter : MonoBehaviour
{
    [SerializeField]
    float firstFootstepDistance = 0.1f;

    [SerializeField]
    float footstepDistance = 1f;

    [SerializeField]
    float velocity0Epsilon = 1e-6f;

    [Header("FMOD")]
    [SerializeField]
    StudioEventEmitter playerWalk;

    [SerializeField]
    StudioEventEmitter playerWalkStart;

    [SerializeField]
    StudioEventEmitter playerWalkStop;

    Vector3 lastPosition;
    Vector3 velocity;

    void Start()
    {
        lastPosition = transform.position;
        StartCoroutine(WalkAsync());
    }

    void FixedUpdate()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    IEnumerator WalkAsync()
    {
        bool first = true;
        bool started = false;
        float distance = 0;

        while (true)
        {
            float velocityMagnitude = velocity.magnitude;
            distance += velocityMagnitude * Time.deltaTime;

            if (velocityMagnitude > velocity0Epsilon && !started)
            {
                if (playerWalkStart != null)
                {
                    playerWalkStart.Play();
                }
                started = true;
                distance = 0;
            }
            if (velocityMagnitude < velocity0Epsilon && !first)
            {
                if (playerWalkStop != null)
                {
                    playerWalkStop.Play();
                }
                first = true;
                started = false;
                distance = 0;
            }
            else if (first && distance > firstFootstepDistance)
            {
                playerWalk.Play();
                first = false;
                distance = 0;
            }
            else if (distance > footstepDistance)
            {
                playerWalk.Play();
                distance = 0;
            }

            yield return null;
        }
    }
}
