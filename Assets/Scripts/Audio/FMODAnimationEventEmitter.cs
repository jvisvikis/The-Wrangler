using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODAnimationEventEmitter : MonoBehaviour
{
    [SerializeField]
    FMODUnity.StudioEventEmitter eventEmitter;

    [SerializeField]
    FMODUnity.StudioEventEmitter eventEmitterStart;

    [SerializeField]
    FMODUnity.StudioEventEmitter eventEmitterStop;

    bool firstOfGame = true;

    public void Emit(AnimationEvent animationEvent)
    {
        if (CheckIsFirstOfGame())
        {
            return;
        }

        if (IsFirstOfLoop(animationEvent))
        {
            eventEmitterStart?.Play();
        }
        else if (animationEvent.animatorClipInfo.weight == 1f)
        {
            eventEmitter.Play();
        }
    }

    public void Idle(AnimationEvent animationEvent)
    {
        if (!CheckIsFirstOfGame() && IsFirstOfLoop(animationEvent))
        {
            eventEmitterStop?.Play();
        }
    }

    bool CheckIsFirstOfGame()
    {
        bool b = firstOfGame;
        firstOfGame = false;
        return b;
    }

    bool IsFirstOfLoop(AnimationEvent animationEvent)
    {
        return animationEvent.animatorStateInfo.normalizedTime < 0.1f;
    }
}
