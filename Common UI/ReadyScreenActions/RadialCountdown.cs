using System;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class RadialCountdown : ReadyScreenAction
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Image radialImage;
    [SerializeField] private CanvasGroup canvasGroup;
    private bool radialFilling = false;
    private float timeStart;
    [SerializeField] private float timeToWait;

    public override Coroutine StartAction(Action successCallback = null, Action<string> failCallback = null)
    {
        return StartCoroutine(Action());
    }

    private void Setup()
    {
        timeStart = Time.timeSinceLevelLoad;
        canvasGroup.alpha = 1.0f;
        radialFilling = true;
        countdownText.text = timeToWait.ToString();
    }

    IEnumerator Action()
    {
        Setup();
        countdownText.transform.DOPunchScale(countdownText.transform.localScale * 0.4f, 1f, 1);
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        countdownText.transform.DOPunchScale(countdownText.transform.localScale * 0.4f, 1f, 1);
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        countdownText.transform.DOPunchScale(countdownText.transform.localScale * 0.4f, 1f, 1);
        yield return new WaitForSeconds(1f);
        countdownText.text = "GO!";
        countdownText.transform.DOPunchScale(countdownText.transform.localScale * 0.6f, 0.25f, 1);
        yield return new WaitForSeconds(0.25f);
        canvasGroup.alpha = 0.0f;
    }

    private void Update()
    {
        if (radialFilling)
        {
            float perc = (Time.timeSinceLevelLoad-timeStart) / timeToWait;
            radialImage.fillAmount = perc;
            if (perc >= 1.0f)
                radialFilling = false;
        }
        
    }
}
