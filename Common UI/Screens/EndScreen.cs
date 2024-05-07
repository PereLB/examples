using UnityEngine;
using System;
using TMPro;
using System.Collections;

public class EndScreen : Modal
{
    #region Variables

    [SerializeField] TextMeshProUGUI score;
    [SerializeField] Element coins;
    [SerializeField] Element drops;
    [SerializeField] Element xp;

    public Action OnRetry;
    public Action<bool> OnEnd;

    private ScoreManager scoreManager;
    private GameSessionManager gameSessionManager;

    #endregion

    #region Lifecycle

    protected override void Awake()
    {
        base.Awake();

        scoreManager = FindAnyObjectByType<ScoreManager>();
        gameSessionManager = FindAnyObjectByType<GameSessionManager>();
    }

    public void OnEnable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreUpdate += UpdateUI;
        }
        if (gameSessionManager != null)
        {
            gameSessionManager.OnScorePayloadPosted += UpdateUIElements;
        }
    }

    private void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreUpdate -= UpdateUI;
        }
        if (gameSessionManager != null)
        {
            gameSessionManager.OnScorePayloadPosted -= UpdateUIElements;
        }
    }

    #endregion

    public void Retry()
    {
        StartCoroutine(WaitForRetry());
    }

    IEnumerator WaitForRetry()
    {
        yield return new WaitForSeconds(0.1f);
        ResetUIElements();
        UIManager.Instance.ShowGroup("ReadyScreen", true);
        if (PetUpdateEventManager.Instance)
        {
            AnalyticManager.TriggerEvent(AnalyticDefinitions.MinigamePlayAgain(PetUpdateEventManager.Instance.m_activeMinigame), true, false);
        }
        OnRetry?.Invoke();
    }

    public void BackToMainMenu()
    {
        OnEnd?.Invoke(false);
    }

    private void UpdateUI(float m_score)
    {
        score.text = m_score.ToString();
        UpdateUIElements();
    }

    private void ResetUIElements()
    {
        xp.Initialize(0);
        xp.gameObject.SetActive(false);
        coins.Initialize(0);
        coins.gameObject.SetActive(false);
        drops.Initialize(0);
        drops.gameObject.SetActive(false);
    }

    private void UpdateUIElements()
    {
        if (gameSessionManager)
        {
            if (gameSessionManager.xpFromSession > 0)
            {
                xp.gameObject.SetActive(true);
                if(gameSessionManager.xpFromSession<1.0f)
                    xp.Initialize(1);
                else
                    xp.Initialize((int)gameSessionManager.xpFromSession);
            }
            if (gameSessionManager.coinsFromSession > 0)
            {
                coins.gameObject.SetActive(true);
                coins.Initialize(gameSessionManager.coinsFromSession);
            }
            if (gameSessionManager.paintsFromSession > 0 || gameSessionManager.wrapsFromSession > 0)
            {
                drops.gameObject.SetActive(true);
                drops.Initialize(gameSessionManager.paintsFromSession + gameSessionManager.wrapsFromSession);
            }
        }
    }
}
