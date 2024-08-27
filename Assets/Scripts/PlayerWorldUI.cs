using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWorldUI : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image pullFillBar;
    [SerializeField] private Image timeFillBar;

    public void SetCanvas(bool active)
    {
        canvas.SetActive(active);
    }
    public void FillPullBar(float fillAmount)
    {
        pullFillBar.fillAmount = fillAmount;
    }

    public void FillTimeBar(float fillAmount)
    {
        timeFillBar.fillAmount = fillAmount;
    }

    public void ResetFillBars()
    {
        pullFillBar.fillAmount = 0f;
        timeFillBar.fillAmount = 0f;
    }
}
