using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Merchant : MonoBehaviour
{
    public List<string> animalsWanted{get; private set;}
    private PlayerController player;
    private string [] animals =  {"Chicken", "Boar", "Cattle", "Horse"};
    private int numShinies;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        animalsWanted = new List<string>();
        animalsWanted.Add(animals[0]);
        UIManager.instance.SetImages(animalsWanted);
    }
    
    public void TakeAnimal(Animal animal)
    {
        if(animalsWanted.Contains(animal.animalName))
        {
            animalsWanted.Remove(animal.animalName);
            animal.MoveTo((Vector2)transform.position, 1f);
            if(animal.isShiny) 
                numShinies++;
            UIManager.instance.SetNumText(animal.animalName);
        }

        if(animalsWanted.Count <= 0) 
        {
            GameManager.instance.AddTime(1+numShinies);
            UIManager.instance.ToggleUpgradePanelState();
            GameManager.instance.pickingUpgrade = true;
        }
    }

    public void AddAnimalsToList()
    {
        numShinies = 0;
        int idx = Random.Range(0,(int)player.strength);
        animalsWanted.Add(animals[idx]);
        UIManager.instance.SetImages(animalsWanted);
    }
 

}
