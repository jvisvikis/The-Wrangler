using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoBelt : MonoBehaviour
{
    [SerializeField] private List<Lasso> lassos;
    [SerializeField] private Lasso lassoPrefab;
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
            Follow(transform);
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

    public void ReleaseLast()
    {
        if(freeIdx == 0)
            return;
        
        freeIdx--;
        lassos[freeIdx].ReleaseAnimal();
    }

    public void ReleaseAll()
    {
        foreach(Lasso lasso in lassos)
        {
            if(!lasso.gotAnimal) return;
            lasso.ReleaseAnimal();
        }
    }

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
            lassos.Add(Instantiate(lassoPrefab, this.transform.position, Quaternion.identity));
    }

    private void Follow(Transform target)
    {
        foreach(Lasso lasso in lassos)
        {
            if(!lasso.isWrangling)
                lasso.Follow(target);
        }
    }   

}
