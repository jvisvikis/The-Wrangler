using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWorldUI : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image pullFillBar;

    public void SetCanvas(bool active)
    {
        canvas.SetActive(active);
    }
    public void FillPullBar(float fillAmount)
    {
        pullFillBar.fillAmount = fillAmount;
    }
    
}
