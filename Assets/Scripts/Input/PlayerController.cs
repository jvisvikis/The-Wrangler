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

    Vector2 GetDirection()
    {
        return playerControls.Player.Move.ReadValue<Vector2>();
    }

    void StartLassoCharge()
    {
        Debug.Log("Start Lasso");
        lassoing = true;
        StartCoroutine(ChargeLasso());
    }

    void ReleaseLasso()
    {
        Debug.Log("Release Lasso");
        lassoing = false;
    }

    public IEnumerator ChargeLasso()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();   
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 Worldpos=Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 dir = new Vector2(Worldpos.x, Worldpos.y).normalized;
        while(lassoing)
        {
            lasso.transform.localPosition = Vector2.Lerp(Vector2.zero, dir*lassoRange, Mathf.PingPong(lassoTimer/lassoChargeTime,1));
            lassoTimer += Time.deltaTime;
            yield return null;
        }
        lassoTimer = 0;
    }
}
