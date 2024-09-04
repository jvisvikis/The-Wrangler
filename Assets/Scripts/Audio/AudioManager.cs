using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    bool startGame = false;

    [SerializeField]
    int upgradeCountForIntensity2 = 3;

    [SerializeField]
    float timeForIntensity3 = 60f;

    [SerializeField]
    FMODUnity.StudioEventEmitter fmodStartGame;

    [SerializeField]
    float delayPersistentStartIfWebGL = 1f;

    int upgradeCount = 0;
    bool reachedTimeForIntensity3 = false;
    float intensity3Timer = 0f;
    bool pausedMethod = false;
    bool pausedFocus = false;

    void Start()
    {
        if (IsWebGL())
        {
            PersistentAudio.Preload();
            StartCoroutine(DelayedStartPersistentAudio());
        }
        else
        {
            StartPersistentAudio();
        }
    }

    IEnumerator DelayedStartPersistentAudio()
    {
        yield return new WaitForSeconds(delayPersistentStartIfWebGL);
        StartPersistentAudio();
    }

    void StartPersistentAudio()
    {
        upgradeCount = 0;
        reachedTimeForIntensity3 = false;
        intensity3Timer = 0f;
        PersistentAudio.StartAudio();
        PersistentAudio.Music.setParameterByName("musicStartGame", startGame ? 1 : 0);
        SetMusicIntensity(0);
        SetPaused(false);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        pausedFocus = !hasFocus;
        UpdatePersistentAudioPaused();
    }

    void Update()
    {
        if (startGame)
        {
            Tick();
        }
    }

    public void PauseIfWebGL()
    {
        if (IsWebGL())
        {
            SetPaused(true);
        }
    }

    bool IsWebGL()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
    }

    void SetPaused(bool paused)
    {
        pausedMethod = paused;
        UpdatePersistentAudioPaused();
    }

    void UpdatePersistentAudioPaused()
    {
        bool paused = pausedMethod || pausedFocus;
        PersistentAudio.Music.setPaused(paused);
        PersistentAudio.Ambience.setPaused(paused);
    }

    public void DidUpgrade()
    {
        upgradeCount++;
        if (reachedTimeForIntensity3)
        {
            // don't downgrade the intensity level
        }
        else if (upgradeCount == upgradeCountForIntensity2)
        {
            SetMusicIntensity(2);
        }
        else if (upgradeCount == 1)
        {
            SetMusicIntensity(1);
        }
    }

    void Tick()
    {
        if (intensity3Timer < timeForIntensity3)
        {
            intensity3Timer += Time.deltaTime;
            if (intensity3Timer >= timeForIntensity3)
            {
                reachedTimeForIntensity3 = true;
                SetMusicIntensity(3);
            }
        }
    }

    void SetMusicIntensity(int intensity)
    {
        PersistentAudio.Music.setParameterByName("musicIntensity", intensity);
    }
}
