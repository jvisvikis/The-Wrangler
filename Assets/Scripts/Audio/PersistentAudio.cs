using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PersistentAudio : MonoBehaviour
{
    [SerializeField]
    EventReference music;

    [SerializeField]
    EventReference ambience;

    public static EventInstance Music { get; private set; }
    public static EventInstance Ambience { get; private set; }

    static PersistentAudio instance;

    public static void StartAudio()
    {
        if (!Music.isValid())
        {
            Music = StartEvent(instance.music);
            Ambience = StartEvent(instance.ambience);
        }
    }

    static EventInstance StartEvent(EventReference e)
    {
        var inst = RuntimeManager.CreateInstance(e);
        inst.start();
        return inst;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }
}
