using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalSpawner : MonoBehaviour
{
    public float rectHeight;
    public float rectWidth;
    
    public void SpawnAnimal(Animal animal)
    {
        Vector2 midPoint = transform.position;
        float x = Random.Range(midPoint.x - rectWidth/2, midPoint.x + rectWidth/2);
        float y = Random.Range(midPoint.y - rectHeight/2, midPoint.y + rectHeight/2);
        Vector2 spawnPoint = new Vector2(x,y);
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (spawnPoint, out navHit, rectWidth, -1);
        Instantiate(animal,navHit.position,Quaternion.identity);
    }
}
