using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoAimer : MonoBehaviour
{
    [SerializeField] private LayerMask animalLayerMask;
    private LassoBelt lassoBelt;
    public Animal animal {get; private set;}

    void Start()
    {
        lassoBelt = FindObjectOfType<LassoBelt>();
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if((animalLayerMask & (1 << other.gameObject.layer)) != 0)
        {
            animal = other.gameObject.GetComponent<Animal>();
            other.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(animal != null)
            animal.GetComponent<SpriteRenderer>().color = Color.white;
            
        animal = null;
    }
}
