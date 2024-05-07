using System;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class Countdown : ReadyScreenAction
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float timeToWait;
    [SerializeField] private float textScale;
    [SerializeField] private float alphaEndValue;
    [SerializeField] private AK.Wwise.Event countTimerSound;
    [SerializeField] private AK.Wwise.Event endTimerSound;

    public override Coroutine StartAction(Action successCallback = null, Action<string> failCallback = null)
    {
        return StartCoroutine(Action());
    }

    private void Setup()
    {
        ResetText();
        countdownText.text = "3";
    }

    private void ResetText()
    {
        countdownText.rectTransform.localScale = Vector3.one;
        canvasGroup.alpha = 0.5f;
    }

    IEnumerator Action()
    {
        Setup();
        countTimerSound.Post(gameObject);
        countdownText.transform.DOScale(countdownText.transform.localScale * textScale, 0.5f);
        canvasGroup.DOFade(alphaEndValue, 0.5f);
        yield return new WaitForSeconds(0.5f);
        ResetText();
        countdownText.text = "2";
        countTimerSound.Post(gameObject);
        countdownText.transform.DOScale(countdownText.transform.localScale * textScale, 0.5f);
        canvasGroup.DOFade(alphaEndValue, 0.5f);
        yield return new WaitForSeconds(0.5f);
        ResetText();
        countdownText.text = "1";
        countTimerSound.Post(gameObject);
        countdownText.transform.DOScale(countdownText.transform.localScale * textScale, 0.5f);
        canvasGroup.DOFade(alphaEndValue, 0.5f);
        yield return new WaitForSeconds(0.5f);
        ResetText();
        countdownText.text = "GO!";
        endTimerSound.Post(gameObject);
        countdownText.transform.DOScale(countdownText.transform.localScale * 2, 0.5f);
        canvasGroup.DOFade(alphaEndValue, 0.5f);
        yield return new WaitForSeconds(0.5f);
        canvasGroup.alpha = 0.0f;
    }
}
