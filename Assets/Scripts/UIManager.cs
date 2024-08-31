using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance {get; private set;}
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Animator upgradePanelAnim;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private List<GameObject> animalsWantedCards;
    [SerializeField] private List<Image> animalsWantedImages;
    [SerializeField] private List<TextMeshProUGUI> animalsWantedNames;
    [SerializeField] private List<TextMeshProUGUI> animalsWantedNums;
    [SerializeField] private List<TextMeshProUGUI> stats;
    [SerializeField] private List<TextMeshProUGUI> animalsDeliveredStatsText;
    [SerializeField] private List<Sprite> animalSprites;
    [SerializeField] private List<string> animalNames;

    [Header("FMOD")]
    [SerializeField] private FMODUnity.StudioEventEmitter fmodUpgradeReveal;
    [SerializeField] private FMODUnity.StudioEventEmitter fmodUpgradeChosen;

    private Dictionary<string,Sprite> animalDictionary = new Dictionary<string, Sprite>();
    private List<string> animalNamesWanted;
    
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
            // DontDestroyOnLoad(this);
            for(int i = 0; i<animalNames.Count; i++)
            {
                animalDictionary.Add(animalNames[i],animalSprites[i]);
            }
        }
    }

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        stats[0].text = $"Speed: {player.speed}";
        stats[1].text = $"Strength: {player.strength}";
        stats[2].text = $"Lassos: {player.lassoBelt.GetNumLassos()}";

        upgradePanelAnim.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        SetTimerText();
    }

    public void SetGamePanel(bool active)
    {
        gamePanel.SetActive(active);
    }

    public void SetNumText(string animalName)
    {
        int idx = animalNamesWanted.IndexOf(animalName);
        if(idx <= -1)
            return;
        
        char num = (char)((animalsWantedNums[idx].text[animalsWantedNums[idx].text.Length-1]) - 1);
        animalsWantedNums[idx].text = $"x{num}";
    }

    public void SetTimerText()
    {
        float timeLeft = GameManager.instance.timeLeft;
        float timeWarning = GameManager.instance.TimeWarning;
        string minutes = $"{(int)timeLeft/60}";
        string seconds = (int)timeLeft%60 >= 10 ? $"{(int)timeLeft%60}" : $"0{(int)timeLeft%60}";
        timerText.color = (int)timeLeft <= timeWarning ? Color.red : Color.white;
        timerText.text = $"{minutes}:{seconds}";
    }

    public void SetImages(List<string> animals)
    {
        Dictionary<string, int> dict= new Dictionary<string, int>();
        for(int i = 0; i < animalNames.Count; i++)
        {
            dict.Add(animalNames[i],0);
            
        }

        foreach(string animal in animals)
        {
            dict[animal]++;
        }

        int imageIdx = 0;
        animalNamesWanted = new List<string>();
        foreach(KeyValuePair<string, int> kvp in dict)
        {
            if(kvp.Value > 0)
            {
                animalsWantedImages[imageIdx].sprite = animalDictionary[kvp.Key];
                animalsWantedNums[imageIdx].text = $"x{kvp.Value}";
                animalsWantedNames[imageIdx].text = kvp.Key;
                animalsWantedCards[imageIdx].gameObject.SetActive(true);
                animalNamesWanted.Add(kvp.Key);
                imageIdx++;
            }
        }

        for(int i = imageIdx; i<animalsWantedCards.Count; i++)
        {
            animalsWantedCards[i].SetActive(false);
        }
    }

    public void SetPathScene(int path)
    {
        FindObjectOfType<SceneReference>().sceneIdx = path;
    }
    public void GameOver()
    {
        AnimalManager animalManager = FindObjectOfType<AnimalManager>();
        animalsDeliveredStatsText[0].text = $"Animals Delivered: {animalManager.animalsDeliveredTotal}";
        int idx = 1;
        foreach(KeyValuePair<string,int> kvp in animalManager.animalsDelivered)
        {
            animalsDeliveredStatsText[idx++].text = $"{kvp.Key} Delivered: {kvp.Value}";
        }
        gameOverPanel.SetActive(true);
    }
    
    public void ToggleUpgradePanelState()
    {
        if(upgradePanelAnim.gameObject.activeSelf)
        {
            StartCoroutine(DelayActiveState(upgradePanelAnim.gameObject, false, 0.5f));
        }
        else
        {
            upgradePanelAnim.gameObject.SetActive(true);
            fmodUpgradeReveal.Play();
        }

        upgradePanelAnim.SetTrigger("SwitchState");
    }

    public void UpgradePlayerStat(string stat)
    {
        GameManager.instance.UpgradePlayerStat(stat);
        stats[0].text = $"Speed: {player.speed}";
        stats[1].text = $"Strength: {player.strength}";
        stats[2].text = $"Lassos: {player.lassoBelt.GetNumLassos()}";
        fmodUpgradeChosen.Play();
    }

    public IEnumerator DelayActiveState(GameObject gameobject, bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        gameobject.SetActive(state);
    }
}
