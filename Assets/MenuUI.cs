using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel;

    void Start()
    {
        controlsPanel.SetActive(false);
    }

    void Update()
    {
        if(Mouse.current.leftButton.isPressed && controlsPanel.activeSelf)
            SetControlsPanel(false);

    }

    public void SetControlsPanel(bool active)
    {
        controlsPanel.SetActive(active);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
