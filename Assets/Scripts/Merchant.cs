using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public List<string> animalsWanted{get; private set;}
    private string [] animals =  {"Chicken", "Boar", "Cattle", "Horse"};

    void Start()
    {
        animalsWanted = new List<string>();
        animalsWanted.Add(animals[2]);
    }
    
    public void TakeAnimal(Animal animal)
    {
        Debug.Log(animal);
        if(animalsWanted.Contains(animal.animalName))
        {
            animalsWanted.Remove(animal.animalName);
            Destroy(animal.gameObject);
        }

        if(animalsWanted.Count <= 0) AddAnimalsToList();
    }

    private void AddAnimalsToList()
    {
        animalsWanted.Add(animals[2]);
    }

}
