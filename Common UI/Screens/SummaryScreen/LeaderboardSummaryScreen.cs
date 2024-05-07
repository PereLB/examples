using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardSummaryScreen : MonoBehaviour
{
    [Header("Listener")]
    [SerializeField] private VoidEventChannelSO skipEvent;

    private Coroutine leaderboard_CO;
    private Coroutine autoskip_CO;
    private Top3LeaderboardManager leaderboardManager;
    
    private bool tapCooldown = false;
    private bool tapped = false;

    private void SubscribeInput()
    {
        if (skipEvent)
            skipEvent.OnEventRaised += SkipButton;
        if (InputManager.Instance)
        {
            InputManager.Instance.OnTap += Tap;
        }
    }

    private void UnsubscribeInput()
    {
        if (skipEvent)
            skipEvent.OnEventRaised -= SkipButton;
        if (InputManager.Instance)
        {
            InputManager.Instance.OnTap -= Tap;
        }
    }

    public void OnDestroy()
    {
        try
        {
            InputManager.Instance.OnTap -= Tap;
        }
        catch { }

        try
        {
            skipEvent.OnEventRaised -= SkipButton;
        }
        catch { }
    }

    public void PopulateScreen(Action m_callback, SummaryScreenData m_data)
    {
        leaderboardManager = GetComponent<Top3LeaderboardManager>();
        leaderboard_CO = StartCoroutine(ActivateLeaderBoards(m_callback, m_data));
        autoskip_CO = StartCoroutine(Autoskip());
    }

    private IEnumerator ActivateLeaderBoards(Action m_callback, SummaryScreenData m_data)
    {
        SubscribeInput();
        if (GameManager.Instance)
        {
            leaderboardManager.InitializeTop3Leaderboard(GameManager.Instance.levelData.type.ToString().ToLower());
        }
        else
        {
            Tap(Vector3.zero);
        }
        yield return new WaitUntil(() => tapped);
        UnsubscribeInput();
        m_callback?.Invoke();
    }

    public void SkipButton()
    {
        UnsubscribeInput();
        if (PetUpdateEventManager.Instance)
        {
            AnalyticManager.TriggerEvent(AnalyticDefinitions.MinigameSummary(PetUpdateEventManager.Instance.m_activeMinigame, true),true,false);
        }
        SceneManager.Instance.LoadHub();
    }

    private void Tap(Vector3 m_pos)
    {
        if (tapCooldown) return;
        if (autoskip_CO!=null)
            StopCoroutine(autoskip_CO);
        tapCooldown = true;
        tapped = true;
    }

    private IEnumerator Autoskip()
    {
        yield return new WaitForSeconds(3.0f);
        Tap(Vector3.zero);
    }
}
