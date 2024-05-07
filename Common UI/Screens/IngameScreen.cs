using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

public class IngameScreen : Modal
{
    #region Variables

    public Action OnPauseGame;
    [SerializeField] IngameLayout ingameLayout = IngameLayout.ScoreOnly;

    [Header("Score only Layout")]
    [SerializeField] private CanvasGroup scoreOnlyCanvasGroup;
    [SerializeField] private TextMeshProUGUI scoreOnOneLayoutNumber;

    [Header("Score and Timer Layout")]
    [SerializeField] private Image radialImage;
    [SerializeField] private CanvasGroup scoreAndTimerCanvasGroup;
    [SerializeField] private TextMeshProUGUI scoreOnTwoLayoutNumber;
    [SerializeField] private TextMeshProUGUI timerOnTwoLayoutNumber;
    public TextMeshProUGUI scoreLayout;
    private float timeToWait;

    [Space]
    [SerializeField] GameObject pauseButton;

    private GameManager gameManager;
    private ScoreManager scoreManager;
    public bool radialFilling = false;
    private float timeStart;

    public bool timerAnimating;
    public bool scoreAnimating;

    #endregion

    #region Lifecycle

    protected override void Awake()
    {
        base.Awake();

        gameManager = GameObject.FindAnyObjectByType<GameManager>();
        scoreManager = GameObject.FindAnyObjectByType<ScoreManager>();

        SetupLayout();
    }

    private void OnEnable()
    {
        ResetScore();

        if (gameManager != null)
        {
            gameManager.OnResetGame += ResetIngameScreen;
            if (ingameLayout == IngameLayout.ScoreAndTimer)
            {
                gameManager.TimerUpdate += UpdateTimer;
                timeToWait = gameManager.gameDuration;
            }
        }

        if (UIManager.Instance)
        {
            UIManager.Instance.OnResumeGame += ResumeGame;
        }

        if (scoreManager != null)
        {
            scoreManager.OnScoreUpdate += UpdateScore;
        }


        //Dotween Ui animation
        timerAnimating = true;
        scoreAnimating = true;

    }

    private void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.OnResetGame -= ResetIngameScreen;
            if (ingameLayout == IngameLayout.ScoreAndTimer)
                gameManager.TimerUpdate -= UpdateTimer;
        }
        if (UIManager.Instance)
        {
            UIManager.Instance.OnResumeGame -= ResumeGame;
        }
        if (scoreManager != null)
        {
            scoreManager.OnScoreUpdate -= UpdateScore;
        }
    }

    #endregion

    private void SetupLayout()
    {
        switch (ingameLayout)
        {
            case IngameLayout.ScoreOnly:
                scoreOnlyCanvasGroup.alpha = 1.0f;
                scoreAndTimerCanvasGroup.alpha = 0.0f;
                break;
            case IngameLayout.ScoreAndTimer:
                scoreOnlyCanvasGroup.alpha = 0.0f;
                scoreAndTimerCanvasGroup.alpha = 1.0f;
                break;
            default:
                scoreOnlyCanvasGroup.alpha = 1.0f;
                scoreAndTimerCanvasGroup.alpha = 0.0f;
                break;
        }
    }

    private void UpdateTimer(float timer)
    {
        timerOnTwoLayoutNumber.text = ((int)timer).ToString();

        if (radialImage)
        {
            float perc = 1.0f - (Mathf.Abs(timer - timeToWait) / timeToWait);
            radialImage.fillAmount = perc;
        }

        if (timer <= 5 && timerAnimating)
        {
            StartCoroutine(TimerEnd());
            timerAnimating = false;
        }
        else
        {
            timerOnTwoLayoutNumber.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    IEnumerator TimerEnd()
    {
        timerOnTwoLayoutNumber.transform.DOScale(timerOnTwoLayoutNumber.transform.localScale * 1.5f, 1f);
        yield return new WaitForSeconds(1f);
        timerOnTwoLayoutNumber.transform.localScale = new Vector3(1, 1, 1);
        timerAnimating = true;
    }

    private void UpdateScore(float score)
    {
        switch (ingameLayout)
        {
            case IngameLayout.ScoreOnly:
                scoreOnOneLayoutNumber.text = score.ToString();
                StartCoroutine(ScoreUpdate(1,score));
                break;
            case IngameLayout.ScoreAndTimer:
                scoreOnTwoLayoutNumber.text = score.ToString();
                StartCoroutine(ScoreUpdate(2,score));
                break;
            default:
                scoreOnOneLayoutNumber.text = score.ToString();
                StartCoroutine(ScoreUpdate(1,score));
                break;
        }
    }
    IEnumerator ScoreUpdate(int layoutIndex, float score)
    {

        if(layoutIndex == 1)
        {
            scoreLayout = scoreOnOneLayoutNumber;
        }
        else
        {
            scoreLayout = scoreOnTwoLayoutNumber;
        }

        if (scoreAnimating && score > 0)
        {
            scoreAnimating = false;
            scoreLayout.transform.DOScale(scoreLayout.transform.localScale * 1.2f, 0.25f);
            yield return new WaitForSeconds(0.25f);
            scoreLayout.transform.DOScale(scoreLayout.transform.localScale * 0.9f, 0.1f);
            yield return new WaitForSeconds(0.1f);
            scoreLayout.transform.localScale = new Vector3(1, 1, 1);
            scoreAnimating = true;
        }
    }

    private void ResetScore()
    {
        UpdateScore(0);
    }

    private void ResetTimer()
    {
        timeStart = Time.timeSinceLevelLoad;
        radialImage.fillAmount = 1.0f;
        UpdateTimer((int)timeToWait);
    }

    private void ResetIngameScreen()
    {
        switch (ingameLayout)
        {
            case IngameLayout.ScoreOnly:
                ResetScore();
                break;
            case IngameLayout.ScoreAndTimer:
                ResetScore();
                ResetTimer();
                break;
            default:
                ResetScore();
                break;
        }
    }

    public void SetLayout(IngameLayout layout) 
    {
        ingameLayout = layout;
        SetupLayout();
    }

    public void PauseGame()
    {
        pauseButton.SetActive(false);
        UIManager.Instance.ShowGroup("PauseScreen");
        OnPauseGame?.Invoke();
    }

    private void ResumeGame()
    {
        pauseButton.SetActive(true);
    }
}

public enum IngameLayout
{
    ScoreOnly, ScoreAndTimer
}
