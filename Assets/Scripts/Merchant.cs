using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Merchant : MonoBehaviour
{
    [SerializeField] private int maxAnimals;
    [SerializeField] private string [] animals =  {"Chicken", "Boar", "Cattle", "Horse"};
    public List<string> animalsWanted{get; private set;}
    private PlayerController player;
    
    private int numShinies;
    private int currentMaxAnimals;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        animalsWanted = new List<string>();
        animalsWanted.Add(animals[0]);
        UIManager.instance.SetImages(animalsWanted);
    }

    void Update()
    {
       
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
            AnimalManager.instance.AnimalDelivered(animal);
            
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
        List<int> uniqueAnimalsIdx = new List<int>();
        for(int i = 0; i<currentMaxAnimals; i++)
        {
            int idx = 0;
            if(uniqueAnimalsIdx.Count >= 4)
            {
                idx = uniqueAnimalsIdx[Random.Range(0,4)];
            }
            else
            {
                idx = Random.Range(0,(int)player.strength);
                uniqueAnimalsIdx.Add(idx);
            }
            
            animalsWanted.Add(animals[idx]);
        }
         
        UIManager.instance.SetImages(animalsWanted);
    }

    public void IncrementCurrentMaxAnimals()
    {
        currentMaxAnimals++;
        if(currentMaxAnimals > maxAnimals)
            currentMaxAnimals = maxAnimals;
    }
 

}
