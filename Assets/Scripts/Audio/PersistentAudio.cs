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

    public static void Preload()
    {
        RuntimeManager.GetEventDescription(instance.music).loadSampleData();
        RuntimeManager.GetEventDescription(instance.ambience).loadSampleData();
    }

    public static void StartAudio()
    {
        if (!Music.isValid())
        {
            Music = StartEvent(instance.music);
            Ambience = StartEvent(instance.ambience);

            if (IsWebGL())
            {
                // From https://qa.fmod.com/t/streaming-music-stems-out-of-sync-when-first-booting-the-game/15112/2
                Music.setProperty(EVENT_PROPERTY.SCHEDULE_DELAY, 16000);
                Music.setProperty(EVENT_PROPERTY.SCHEDULE_LOOKAHEAD, 48000);
                // If this still isn't working (music layers getting out of sync), the last thing I
                // can think of is to pause the audio when the game is backgrounded, but since
                // that's the default behaviour anyway, not sure how easy/effective that would be.
            }
        }
    }

    static bool IsWebGL()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
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

    void Start()
    {
        RuntimeManager.GetEventDescription(music).loadSampleData();
        RuntimeManager.GetEventDescription(ambience).loadSampleData();
    }
}
