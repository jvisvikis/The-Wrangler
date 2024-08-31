using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    [SerializeField] private LayerMask animalLayerMask;
    [SerializeField] private GameObject lassoCard;
    [SerializeField] private float followSharpness = 0.05f;
    
    public Animal animal {get; private set;}
    public bool gotAnimal{get; private set;}
    public bool isWrangling{get; set;}

    private Rigidbody2D rb2d;
    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void SetAnimal(Animal animal)
    {
        this.animal = animal;
    }

    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
    }

    public void AnimalWrangled()
    {
        lassoCard.SetActive(false);
        animal.Wrangled();
    }

    public void BringAnimal(Vector2 dir)
    {
        gotAnimal = true;
        transform.position = animal.transform.position;
        transform.parent = animal.transform;
        animal.PullBack(-dir, this.gameObject);
        animal.EnterBeingWrangledState();
    }

    public void ReleaseAnimal(bool isFree)
    {
        lassoCard.SetActive(true);
        gotAnimal = false;
        isWrangling = false;
        transform.parent = player.transform.parent;
        animal.GetComponent<SpriteRenderer>().color = Color.white;
        animal.Release(isFree);
        animal = null;
    }

    public void Follow(Vector2 target)
    {
        if(animal != null)
            animal.Follow(target);
        else
            transform.position = Vector2.Lerp(transform.position, target, Time.deltaTime*followSharpness);   
        
    }
}
