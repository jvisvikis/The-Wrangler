using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager instance;
    [SerializeField] private List<Animal> animalPrefabs;
    [SerializeField] private List<Animal> shinyPrefabs;
    [SerializeField] private List<AnimalSpawner> animalSpawners;
    [SerializeField] private int maxNumAnimals;
    [SerializeField] private int minNumAnimal;
    [SerializeField] private float shinyChance;

    [Header("FMOD")]
    [SerializeField] private FMODUnity.StudioEventEmitter fmodAnimalDelivered;
    
    private Dictionary<string,int> animalsActive;
    public Dictionary<string,int> animalsDelivered;
    public int animalsDeliveredTotal {get; private set;}

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
        animalsDelivered = new Dictionary<string, int>();
        foreach(Animal animal in animalPrefabs)
        {
            animalsActive.Add(animal.animalName,0);
            animalsDelivered.Add(animal.animalName,0);
        }

        for(int i = 0; i<maxNumAnimals; i++)
        {
            bool spawnShiny = Random.Range(0.0f,1.0f) <= shinyChance;
            if(!spawnShiny)
                animalSpawners[i%animalSpawners.Count].SpawnAnimal(animalPrefabs[i%animalPrefabs.Count].gameObject);
            else    
                animalSpawners[i%animalSpawners.Count].SpawnAnimal(shinyPrefabs[i%shinyPrefabs.Count].gameObject);
                
            animalsActive[animalPrefabs[i%animalPrefabs.Count].animalName]++;
        }
    }

    public void AnimalDelivered(Animal animal)
    {
        bool spawnShiny = Random.Range(0.0f,1.0f) <= shinyChance;
        animalsDelivered[animal.animalName]++;
        animalsDeliveredTotal++;
        for(int i = 0; i<animalPrefabs.Count; i++)
        {
            if(animal.animalName.Contains(animalPrefabs[i].animalName))
            {
                if(!spawnShiny)
                    animalSpawners[i%animalSpawners.Count].SpawnAnimal(animalPrefabs[i].gameObject);
                else
                    animalSpawners[i%animalSpawners.Count].SpawnAnimal(shinyPrefabs[Random.Range(0,shinyPrefabs.Count)].gameObject);
            }
        }

        fmodAnimalDelivered.Play();
        fmodAnimalDelivered.EventInstance.setParameterByName("animalType", animal.AnimalTypeValue);
    }

}
