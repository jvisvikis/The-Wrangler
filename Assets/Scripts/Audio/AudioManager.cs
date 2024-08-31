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

    public static void Tick()
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

    public static void SetStartGame(bool startGame)
    {
        instance.music.SetParameter("musicStartGame", startGame ? 1f : 0f);
    }

    static void SetMusicIntensity(int intensity)
    {
        instance.music.SetParameter("musicIntensity", intensity);
    }
}
