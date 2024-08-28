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
        animal.Wrangled();
    }

    public void BringAnimal(Vector2 dir)
    {
        gotAnimal = true;
        transform.position = animal.transform.position; // change this to move towards position overtime
        animal.transform.parent = transform;
        animal.transform.localPosition = Vector2.zero;
        animal.PullBack(-dir, this.gameObject);
        animal.EnterBeingWrangledState();
    }

    public void ReleaseAnimal(bool isFree)
    {
        gotAnimal = false;
        isWrangling = false;
        animal.transform.parent = null;
        animal.GetComponent<SpriteRenderer>().color = Color.white;
        animal.Release(isFree);
        animal = null;
    }

    public void Follow(Vector2 target)
    {
       
        transform.position = Vector2.Lerp(transform.position, target, Time.deltaTime*followSharpness);   
        
    }
}
