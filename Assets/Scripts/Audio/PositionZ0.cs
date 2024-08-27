using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For FMOD 2d spatial audio it helps to have the FMOD Studio Listener at z = 0
/// i.e. on the same z plane as the audio sources.
/// </summary>
public class PositionZ0 : MonoBehaviour
{
    void Start()
    {
        SetPositionZ0();
    }

    void Update()
    {
        SetPositionZ0();
    }

    void SetPositionZ0()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }
}
