using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Merchant : MonoBehaviour
{
    [SerializeField] private Image animalWantedImage;
    [SerializeField] private TextMeshProUGUI animalNameText;
    public List<string> animalsWanted{get; private set;}
    public List<Sprite> animalSprites;
    private PlayerController player;
    private string [] animals =  {"Chicken", "Boar", "Cattle", "Horse"};
    

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        animalsWanted = new List<string>();
        animalsWanted.Add(animals[0]);
        SetAnimalImage(0);
        SetAnimalName(animals[0]);
    }
    
    public void TakeAnimal(Animal animal)
    {
        if(animalsWanted.Contains(animal.animalName))
        {
            animalsWanted.Remove(animal.animalName);
            animal.MoveTo((Vector2)transform.position, 1f);
        }

        if(animalsWanted.Count <= 0) AddAnimalsToList();
    }

    private void AddAnimalsToList()
    {
        int idx = Random.Range(0,(int)player.pullStrength);
        animalsWanted.Add(animals[idx]);
        SetAnimalImage(idx);
        SetAnimalName(animals[idx]);
    }

    private void SetAnimalImage(int idx)
    {
        animalWantedImage.sprite = animalSprites[idx];
    }

    private void SetAnimalName(string text)
    {
        animalNameText.text = text;
    }
 

}
