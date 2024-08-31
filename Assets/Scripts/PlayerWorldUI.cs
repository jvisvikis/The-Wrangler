using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWorldUI : MonoBehaviour
{
    [SerializeField] private GameObject wrangleCanvas;
    [SerializeField] private GameObject releaseCanvas;
    [SerializeField] private Image pullFillBar;
    [SerializeField] private Image timeFillBar;

    public void Start()
    {
        SetCanvas(false);
        SetReleaseCanvas(false);
    }
    public void SetCanvas(bool active)
    {
        wrangleCanvas.SetActive(active);
    }

    public void SetReleaseCanvas(bool active)
    {
        releaseCanvas.SetActive(active);
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
