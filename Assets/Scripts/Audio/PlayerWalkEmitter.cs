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

    // [SerializeField]
    // StudioEventEmitter playerWalkStart;

    // [SerializeField]
    // StudioEventEmitter playerWalkStop;

    void Start()
    {
        StartCoroutine(WalkAsync());
    }

    IEnumerator WalkAsync()
    {
        bool first = true;
        float distance = 0;
        var rb = GetComponentInParent<Rigidbody2D>();

        while (true)
        {
            float velocity = rb.velocity.magnitude;
            distance += velocity * Time.deltaTime;

            if (velocity < velocity0Epsilon && !first)
            {
                // playerWalkStop.Play();
                first = true;
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
