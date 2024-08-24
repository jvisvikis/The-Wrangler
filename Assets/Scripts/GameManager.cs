using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set;}
    public bool pickingUpgrade {get; set;}
    private float startTime;
    private float timer => Time.time - startTime;

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
        }
    }

    void Start()
    {
        merchant = FindObjectOfType<Merchant>();
        player = FindObjectOfType<PlayerController>();
    }

    public void UpgradePlayerStat(string stat)
    {
        player.UpgradePlayerStat(stat);
        merchant.AddAnimalsToList();
        pickingUpgrade = false;
    }

}
