using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LassoWindUpState : State
{
    [SerializeField] private Lasso lasso;
    [SerializeField] private float lassoRange = 5f;
    [SerializeField] private float lassoChargeTime = 1f;    

    private float lassoTimer;
    private bool lassoing;

   public override void Enter()
   {
        isComplete = false;
        StartCoroutine(ChargeLasso());
   }

   public override void Exit()
   {
        StartCoroutine(BringLassoBack(0.5f,0.25f));
   }

   private IEnumerator ChargeLasso()
    {
        lassoing = true;
        Vector3 mousePos = Mouse.current.position.ReadValue();   
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldPos=Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 dir = ((Vector2)worldPos - (Vector2)transform.position).normalized;
        while(lassoing)
        {
            lasso.transform.localPosition = Vector2.Lerp(Vector2.zero, dir*lassoRange, Mathf.PingPong(lassoTimer/lassoChargeTime,1));
            lassoTimer += Time.deltaTime;
            yield return null;
        }
        lassoTimer = 0;
    }

    private IEnumerator BringLassoBack(float duration, float delay)
    {
        Vector2 startPos = lasso.transform.localPosition;
        float timer = 0;
        yield return new WaitForSeconds(delay);
        while(timer < duration)
        {
            float stepAmount = Mathf.Pow (timer * 1.5f, 2);
            lasso.transform.localPosition = Vector2.Lerp(lasso.transform.localPosition, Vector2.zero, stepAmount);
            timer += Time.deltaTime;
            
            yield return null;
        }
        lasso.transform.localPosition = Vector2.zero;
        lassoTimer = 0;
        isComplete = true;
    }

}
