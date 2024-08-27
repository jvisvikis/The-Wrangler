using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]
public class RandomiseParameters : MonoBehaviour
{
    [SerializeField]
    string[] parameters;

    // [SerializeField]
    // float minValue;

    [SerializeField]
    float maxValue;

    [SerializeField]
    float minInterval;

    [SerializeField]
    float maxInterval;

    void Start()
    {
        StartCoroutine(AsyncLoop());
    }

    IEnumerator AsyncLoop()
    {
        var emitter = GetComponent<StudioEventEmitter>();

        while (true)
        {
            float nextInterval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(nextInterval);

            float total = 0f;
            var parameterValues = new Dictionary<string, float>();

            foreach (var param in parameters)
            {
                float value = Random.Range(0f, maxValue);
                total += value;
                parameterValues[param] = value;
            }

            foreach (var kv in parameterValues)
            {
                emitter.SetParameter(kv.Key, kv.Value / total);
            }
        }
    }
}
