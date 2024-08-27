using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (AnimalSpawner))]
public class AnimalSpawnerEditor : Editor
{
   void OnSceneGUI()
   {
        AnimalSpawner animalSpawner = (AnimalSpawner)target;
        Handles.color = Color.green;
        if(animalSpawner.transform != null)
        {
            Vector3 [] verts = new Vector3 []
            {
                new Vector3(animalSpawner.transform.position.x - animalSpawner.rectWidth/2, animalSpawner.transform.position.y - animalSpawner.rectHeight/2, 0),
                new Vector3(animalSpawner.transform.position.x + animalSpawner.rectWidth/2, animalSpawner.transform.position.y - animalSpawner.rectHeight/2, 0),
                new Vector3(animalSpawner.transform.position.x + animalSpawner.rectWidth/2, animalSpawner.transform.position.y + animalSpawner.rectHeight/2, 0),
                new Vector3(animalSpawner.transform.position.x - animalSpawner.rectWidth/2, animalSpawner.transform.position.y + animalSpawner.rectHeight/2, 0)
                
            };
            Handles.DrawSolidRectangleWithOutline(verts, new Color(0f,1f,0f,0.1f), Color.black);
        }
   }
}
