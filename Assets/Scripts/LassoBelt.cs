using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoBelt : MonoBehaviour
{
    [SerializeField] private List<Lasso> lassos;
    [SerializeField] private Lasso lassoPrefab;
    [SerializeField] private Transform lassoHome;
    [SerializeField] private float distFromLassoHome;
    [SerializeField] private int lassoLimit = 4;

    public bool followPlayer {get; set;}
    private int freeIdx;

    void Start()
    {
        followPlayer = true;
    }

    void FixedUpdate()
    {
        if(followPlayer)
            Follow(lassoHome);
    }

    public int GetNumLassos()
    {
        return lassos.Count;
    }
    
    public List<string> GetAnimalNames()
    {
        return null;
    }
    public Lasso GetLastInUse()
    {
        if(freeIdx == 0)
            return null;
        
        return lassos[freeIdx-1];
    }

    public Lasso GetFreeLasso()
    {
        if(freeIdx < lassos.Count)
            return lassos[freeIdx];

        return null;
    }

    public Vector2 GetLassoPos(int idx)
    {
        return (Vector2)lassos[idx].transform.position;
    }

    public void ReleaseLast(bool isFree)
    {
        if(freeIdx == 0)
            return;
        
        freeIdx--;
        lassos[freeIdx].ReleaseAnimal(isFree);
    }

    // public void ReleaseAll()
    // {
    //     foreach(Lasso lasso in lassos)
    //     {
    //         if(!lasso.gotAnimal) return;
    //         lasso.ReleaseAnimal();
    //     }
    // }

    public void BringAnimal(Vector2 dir)
    {
        lassos[freeIdx].BringAnimal(dir);
    }

    public void GrabAnimal()
    {
        lassos[freeIdx].AnimalWrangled();
        lassos[freeIdx].isWrangling = false;
        freeIdx++;
    }

    public void AddLasso()
    {
        if(lassos.Count < lassoLimit)
        {
            lassos.Add(Instantiate(lassoPrefab, this.transform.position, Quaternion.identity));
            lassos[lassos.Count-1].transform.parent = transform.parent;
        }

    }

    private void Follow(Transform target)
    {

        for(int i = 0; i< lassos.Count; i++)
        {
            Vector2 yOffset = new Vector2(0,distFromLassoHome - (i+1)*distFromLassoHome*2/(lassos.Count+1));
            lassos[i].Follow((Vector2)target.position + yOffset);
        }
    }   

}
