using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance {get; private set;}
    [SerializeField] private List<Image> animalsWanted;
    [SerializeField] private List<TextMeshProUGUI> animalsWantedNums;
    [SerializeField] private List<Sprite> animalSprites;
    [SerializeField] private List<string> animalNames;

    private Dictionary<string,Sprite> animalDictionary = new Dictionary<string, Sprite>();
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
            for(int i = 0; i<animalNames.Count; i++)
            {
                animalDictionary.Add(animalNames[i],animalSprites[i]);
            }
        }
    }

    public void SetImages(List<string> animals)
    {
        Dictionary<string, int> dict= new Dictionary<string, int>();
        for(int i = 0; i < animalNames.Count; i++)
        {
            dict.Add(animalNames[i],0);
            if(i >= animals.Count)
                animalsWanted[i].gameObject.SetActive(false);
        }

        foreach(string animal in animals)
        {
            dict[animal]++;
        }

        int imageIdx = 0;
        foreach(KeyValuePair<string, int> kvp in dict)
        {
            if(kvp.Value > 0)
            {
                animalsWanted[imageIdx].sprite = animalDictionary[kvp.Key];
                animalsWantedNums[imageIdx].text = $"x{kvp.Value}";
                imageIdx++;
            }
        }
    }
}
