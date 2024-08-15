using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    [SerializeField] private LayerMask animalLayerMask;
    public GameObject animal {get; private set;}

    private bool gotAnimal;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if((animalLayerMask & (1 << other.gameObject.layer)) != 0)
        {
            animal = other.gameObject;
            animal.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(animal != other.gameObject || gotAnimal) return;
        animal.GetComponent<SpriteRenderer>().color = Color.white;
        animal = null;
    }

    public void BringAnimal()
    {
        gotAnimal = true;
        animal.transform.parent = transform;
        animal.transform.localPosition = Vector2.zero;
    }
}
