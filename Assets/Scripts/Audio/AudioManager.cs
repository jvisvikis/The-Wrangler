using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    FMODUnity.StudioEventEmitter music;

    [SerializeField]
    int upgradeCountForIntensity2 = 3;

    [SerializeField]
    float timeForIntensity3 = 60f;

    static AudioManager instance;
    static bool startedGame = false;
    static int upgradeCount = 0;
    static bool reachedTimeForIntensity3 = false;
    static float intensity3Timer;

    void Awake()
    {
        if (instance == null)
        {
            GameObject.DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    void Update()
    {
        if (startedGame)
        {
            Tick();
        }
    }

    public static void StartGame()
    {
        instance.music.SetParameter("musicStartGame", 1f);
        startedGame = true;
        upgradeCount = 0;
        reachedTimeForIntensity3 = false;
        intensity3Timer = 0;
    }

    public static void DidUpgrade()
    {
        upgradeCount++;
        if (reachedTimeForIntensity3)
        {
            // don't downgrade the intensity level
        }
        else if (upgradeCount == instance.upgradeCountForIntensity2)
        {
            SetMusicIntensity(2);
        }
        else if (upgradeCount == 1)
        {
            SetMusicIntensity(1);
        }
    }

    static void Tick()
    {
        if (intensity3Timer < instance.timeForIntensity3)
        {
            intensity3Timer += Time.deltaTime;
            if (intensity3Timer >= instance.timeForIntensity3)
            {
                reachedTimeForIntensity3 = true;
                SetMusicIntensity(3);
            }
        }
    }

    static void SetMusicIntensity(int intensity)
    {
        instance.music.SetParameter("musicIntensity", intensity);
    }
}
