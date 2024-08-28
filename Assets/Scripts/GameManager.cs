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
    public float timeElapsed => Time.time - startTime;
    public float timeLeft => Mathf.Max(0f,timeGiven - timeElapsed + extraTime);

    public float addTimer => timeElapsed - addStartTime;
    public bool gameOver => timeLeft <= 0;
    private float startTime;
    private float addStartTime;
    private float extraTime;

    private Merchant merchant;
    private PlayerController player;
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
    }

    void Update()
    {
        if(addTimer >= timeToAdd)
        {
            addStartTime = timeElapsed;
            merchant.IncrementCurrentMaxAnimals();
        }
    }

    public void UpgradePlayerStat(string stat)
    {
        player.UpgradePlayerStat(stat);
        merchant.AddAnimalsToList();
        pickingUpgrade = false;
    }

    public void AddTime(int modifierMultiplier)
    {
        extraTime += extraTimeModifier*modifierMultiplier;
    }

}
