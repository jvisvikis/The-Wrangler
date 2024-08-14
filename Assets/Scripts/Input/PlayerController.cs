using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Controls playerControls;
    private Rigidbody2D rb2d;

    [SerializeField] float speed = 2f;

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
        Debug.Log(GetDirection() * speed);
        rb2d.velocity = GetDirection() * speed ;
    }

    Vector2 GetDirection()
    {
        return playerControls.Player.Move.ReadValue<Vector2>();
    }

    void StartLassoCharge()
    {
        Debug.Log("Start Lasso");
    }

    void ReleaseLasso()
    {
        Debug.Log("Release Lasso");
    }
}
