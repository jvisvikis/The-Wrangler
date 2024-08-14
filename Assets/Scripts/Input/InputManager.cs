using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance {get; private set;}

    private Controls playerControls;
     
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        playerControls = new Controls();
        playerControls.Player.Wrangle.canceled += ctx => Debug.Log("Released");
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playerControls.Player.Move.ReadValue<Vector2>();
    }

    public bool PlayerStartedWrangle()
    {
        return playerControls.Player.Wrangle.triggered;
    }

}
