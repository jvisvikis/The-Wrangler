using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Controls playerControls;
    private Rigidbody2D rb2d;

    [SerializeField] private GameObject lasso;
    [SerializeField] private float lassoRange = 5f;
    [SerializeField] private float lassoChargeTime = 1f;
    [SerializeField] private float speed = 2f;
    
    private float lassoTimer;
    private bool lassoing;
    private bool bringingBackLasso;

    void Awake()
    {
        playerControls = new Controls();
        playerControls.Player.Wrangle.performed += ctx => StartLassoCharge();
        playerControls.Player.Wrangle.canceled += ctx => ReleaseLasso();

        rb2d = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {
        if(!lassoing) rb2d.velocity = GetDirection() * speed;
        else rb2d.velocity = Vector2.zero;
    }

    public Vector2 GetDirection()
    {
        return playerControls.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLassoPosition()
    {
        return (Vector2)lasso.transform.position;
    }

    void StartLassoCharge()
    {
        Debug.Log("Start Lasso");
        lassoing = true;
        if(!bringingBackLasso) StartCoroutine(ChargeLasso());
    }

    void ReleaseLasso()
    {
        Debug.Log("Release Lasso");
        lassoing = false;
        if (!bringingBackLasso) StartCoroutine(BringLassoBack(0.5f, 0.25f));
    }

    private IEnumerator ChargeLasso()
    {
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
        
    }

    private IEnumerator BringLassoBack(float duration, float delay)
    {
        bringingBackLasso = true;
        Vector2 startPos = lasso.transform.localPosition;
        float timer = 0;
        yield return new WaitForSeconds(delay);
        while(timer < duration)
        {
            float stepAmount = Mathf.Pow (timer * duration, 2);
            lasso.transform.localPosition = Vector2.Lerp(lasso.transform.localPosition, Vector2.zero, stepAmount);
            timer += Time.deltaTime;
            
            yield return null;
        }
        lasso.transform.localPosition = Vector2.zero;
        lassoTimer = 0;
        Debug.Log("lasso is back");
        bringingBackLasso = false;
    }
}
