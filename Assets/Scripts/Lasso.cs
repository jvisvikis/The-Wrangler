using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    [SerializeField] private LayerMask animalLayerMask;
    
    public Animal animal {get; private set;}

    public bool gotAnimal{get; private set;}
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if((animalLayerMask & (1 << other.gameObject.layer)) != 0)
        {
            animal = other.gameObject.GetComponent<Animal>();
            animal.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(animal == null || gotAnimal) return;
        animal.GetComponent<SpriteRenderer>().color = Color.white;
        animal = null;
    }

    public void AnimalWrangled()
    {
        animal.Wrangled();
    }

    public void BringAnimal(Vector2 dir)
    {
        gotAnimal = true;
        transform.position = animal.transform.position;
        animal.transform.parent = transform;
        animal.transform.localPosition = Vector2.zero;
        animal.PullBack(-dir, this.gameObject);
    }

    public void ReleaseAnimal()
    {
        gotAnimal = false;
        animal.transform.parent = null;
        animal.GetComponent<SpriteRenderer>().color = Color.white;
        animal.Release();
        animal = null;

    }
}
