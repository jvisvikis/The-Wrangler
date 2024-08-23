using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    [SerializeField] private LayerMask animalLayerMask;
    [SerializeField] private float followDist = 2;
    [SerializeField] private float followSharpness = 0.05f;
    
    public Animal animal {get; private set;}
    public bool gotAnimal{get; private set;}
    public bool isWrangling{get; set;}

    private Rigidbody2D rb2d;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other);
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
        isWrangling = false;
        animal.transform.parent = null;
        animal.GetComponent<SpriteRenderer>().color = Color.white;
        animal.Release();
        animal = null;
    }

    public void Follow(Transform target)
    {
        if(Vector2.Distance(target.position, transform.position) > followDist)
        {
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime*followSharpness);
            
        }
    }
}
