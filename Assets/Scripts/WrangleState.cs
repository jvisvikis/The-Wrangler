using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrangleState : State
{
    public Lasso lasso;
    public bool animalFollowing;
    public override void Enter()
    {
        isComplete = false;
        StartCoroutine(BringAnimalBack());
    }

    public override void Do()
    {

    }

    private IEnumerator BringAnimalBack()
    {
        
        Vector2 startPos = (Vector2)lasso.transform.localPosition;
        float timer = 0;
        lasso.BringAnimal();
        while(Vector2.Distance(lasso.transform.position, transform.position) > 2f && lasso.animal != null)
        {
            lasso.transform.localPosition = Vector2.Lerp(startPos, Vector2.zero, timer);
            timer -= Time.deltaTime;
            // if(playerControls.Player.Wrangle.triggered) timer += 0.15f;        
            yield return null;
        }
        lasso.transform.parent = null;
        isComplete = true;
        animalFollowing = true;
    }
}
