using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager instance;
    [SerializeField] private List<Animal> animalPrefabs;
    [SerializeField] private List<AnimalSpawner> animalSpawners;
    [SerializeField] private int maxNumAnimals;
    
    private Dictionary<string,int> animalsActive;
    private int activeAnimalCount;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animalsActive = new Dictionary<string, int>();
        foreach(Animal animal in animalPrefabs)
        {
            animalsActive.Add(animal.animalName,0);
        }

        for(int i = 0; i<maxNumAnimals; i++)
        {
            animalSpawners[i%animalSpawners.Count].SpawnAnimal(animalPrefabs[i%animalPrefabs.Count]);
            animalsActive[animalPrefabs[i%animalPrefabs.Count].animalName]++;
            activeAnimalCount++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
