using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set;}
    public bool pickingUpgrade {get; set;}
    [SerializeField] private float extraTimeModifier;
    [SerializeField] private float timeGiven;
    [SerializeField] private float timeToAdd;
    [SerializeField] float timeWarning = 20f;
    [SerializeField] float timeDireWarning = 3f;
    [SerializeField] float timeExtremelyDireWarning = 1f;

    [Header("FMOD")]
    [SerializeField] float direWarningTickPeriod = 0.2f;
    [SerializeField] float extremelyDireWarningTickPeriod = 0.25f;
    [SerializeField] FMODUnity.StudioEventEmitter fmodRoundCountdownStart;
    [SerializeField] FMODUnity.StudioEventEmitter fmodRoundCountdownTick;
    [SerializeField] FMODUnity.StudioEventEmitter fmodRoundCountdownTickFast;
    [SerializeField] FMODUnity.StudioEventEmitter fmodGameOver;

    public float timeElapsed => Time.time - startTime;
    public float timeLeft => Mathf.Max(0f,timeGiven - timeElapsed + extraTime);
    public float TimeWarning => timeWarning;

    public float addTimer => timeElapsed - addStartTime;
    public bool gameOver => timeLeft <= 0;
    private float startTime;
    private float addStartTime;
    private float extraTime;

    private Merchant merchant;
    private PlayerController player;
    private AudioManager audioManager;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            startTime = Time.time;
            addStartTime = startTime;
        }
    }

    void Start()
    {
        merchant = FindObjectOfType<Merchant>();
        player = FindObjectOfType<PlayerController>();
        audioManager = FindObjectOfType<AudioManager>();
        StartCoroutine(AsyncFMODEmitter());
    }

    void Update()
    {
        if(addTimer >= timeToAdd)
        {
            addStartTime = timeElapsed;
            merchant.IncrementCurrentMaxAnimals();
        }

        if(gameOver)
            GameOverSequence();

    }

    public void GameOverSequence()
    {
        UIManager.instance.GameOver();
    }

    public void UpgradePlayerStat(string stat)
    {
        player.UpgradePlayerStat(stat);
        merchant.AddAnimalsToList();
        pickingUpgrade = false;
        audioManager.DidUpgrade();
    }

    public void AddTime(int modifierMultiplier)
    {
        StartCoroutine(UIManager.instance.AddTimeUI(extraTimeModifier*modifierMultiplier));
        extraTime += extraTimeModifier*modifierMultiplier;
    }

    private IEnumerator AsyncFMODEmitter()
    {
        float prevTimeLeft = timeLeft;

        while (!gameOver)
        {
            if (timeLeft < timeWarning && ((int)timeLeft) != ((int)prevTimeLeft))
            {
                if (Mathf.Round(timeLeft) == Mathf.Round(timeWarning))
                {
                    fmodRoundCountdownStart.Play();
                }
                fmodRoundCountdownTick.Play();
                if (timeLeft < timeExtremelyDireWarning)
                {
                    StartCoroutine(PlayDireWarningsFor1Second(extremelyDireWarningTickPeriod));
                }
                else if (timeLeft < timeDireWarning)
                {
                    StartCoroutine(PlayDireWarningsFor1Second(direWarningTickPeriod));
                }
            }
            prevTimeLeft = timeLeft;
            yield return null;
        }

        fmodGameOver.Play();
    }

    private IEnumerator PlayDireWarningsFor1Second(float period)
    {
        int numWarnings = (int)Mathf.Round(1f / period);
        for (int i = 0; i < numWarnings; i++)
        {
            fmodRoundCountdownTickFast.Play();
            yield return new WaitForSeconds(period);
        }
    }
}
