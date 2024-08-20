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
    }

    public override void Do()
    {

    }
}
