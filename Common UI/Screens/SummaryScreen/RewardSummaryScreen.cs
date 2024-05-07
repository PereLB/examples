using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSummaryScreen : MonoBehaviour
{
    [SerializeField] private UpdateElementSummary UpdateCanvasElement;

    [Header("Listener")]
    [SerializeField] private VoidEventChannelSO skipEvent;

    [Header("Broadcasters")]
    [SerializeField] private ElementChannelSO startUpdateElementEvent;
    [SerializeField] private ElementChannelSO endUpdateElementEvent;

    [Header("Audio Events")]
    [SerializeField] AK.Wwise.Event funEventSound;
    [SerializeField] AK.Wwise.Event foodEventSound;
    [SerializeField] AK.Wwise.Event careEventSound;
    [SerializeField] AK.Wwise.Event coinEventSound;

    private Coroutine rewards_CO;

    //current add element control variables
    private Coroutine currentAddElement_CO;
    private bool addElementDone;
    private RewardType currentRewardType;
    private int currentRewardValue;

    private void SubscribeInput()
    {
        if (InputManager.Instance)
        {
            InputManager.Instance.OnTap += Tap;
        }
    }

    private void UnsubscribeInput()
    {
        if (InputManager.Instance)
        {
            InputManager.Instance.OnTap -= Tap;
        }
    }

    public void PopulateScreen(Action m_callback, SummaryScreenData m_data)
    {
        rewards_CO = StartCoroutine(AddRewards(m_callback, m_data));
    }

    private IEnumerator AddRewards(Action m_callback, SummaryScreenData m_data)
    {
        if (skipEvent)
            skipEvent.OnEventRaised += SkipButton;
        yield return StartCoroutine(AddStats(m_data.care, m_data.excitement, m_data.hunger));
        if (m_data.coinEarned > 0)
        {
            currentAddElement_CO = StartCoroutine(AddElement(RewardType.Coin, m_data.coinEarned));
            yield return new WaitUntil(() => addElementDone);
        }
        yield return AddDrops(m_data.itemsFromServer);
        yield return new WaitForSeconds(2.0f);
        if (skipEvent)
            skipEvent.OnEventRaised -= SkipButton;
        m_callback?.Invoke();
    }

    private IEnumerator AddStats(float m_care, float m_excitement, float m_hunger)
    {
        if (m_care > 0)
        {
            currentAddElement_CO = StartCoroutine(AddElement(RewardType.Care, m_care));
            yield return new WaitUntil(()=>addElementDone);
        }
        if (m_excitement > 0)
        {
            currentAddElement_CO = StartCoroutine(AddElement(RewardType.Excitement, m_excitement));
            yield return new WaitUntil(() => addElementDone);
        }
        if (m_hunger > 0)
        {
            currentAddElement_CO = StartCoroutine(AddElement(RewardType.Hunger, m_hunger));
            yield return new WaitUntil(() => addElementDone);
        }
    }

    private IEnumerator AddDrops(List<DropItems> m_drops)
    {
        if (m_drops.Count > 0)
        {
            foreach (var drop in m_drops)
            {
                currentAddElement_CO = StartCoroutine(AddElement((RewardType)drop.id, drop.amount));
                yield return new WaitUntil(() => addElementDone);
            }
        }
    }

    private IEnumerator AddElement(RewardType m_type, float m_value)
    {

        addElementDone = false;
        currentRewardType = m_type;
        currentRewardValue = (int)m_value;
        PlayEvent(m_type);
        if (startUpdateElementEvent)
            startUpdateElementEvent.RaiseEvent(m_type, (int)m_value);
        yield return UpdateCanvasElement.AddElement((int)m_type, (int)m_value);
        if (endUpdateElementEvent)
            endUpdateElementEvent.RaiseEvent(m_type, (int)m_value);
        addElementDone = true;
    }

    private void Tap(Vector3 m_pos)
    {
        if (currentAddElement_CO != null)
        {
            StopCoroutine(currentAddElement_CO);
            UpdateCanvasElement.StopElementAnimation();
            if (endUpdateElementEvent)
                endUpdateElementEvent.RaiseEvent(currentRewardType, currentRewardValue);
            addElementDone = true;
        }
    }

    private void SkipButton()
    {
        if (skipEvent)
            skipEvent.OnEventRaised -= SkipButton;
        if (PetUpdateEventManager.Instance)
        {
            AnalyticManager.TriggerEvent(AnalyticDefinitions.MinigameSummary(PetUpdateEventManager.Instance.m_activeMinigame, true), true, false);
        }
        SceneManager.Instance.LoadHub();
    }

    private void PlayEvent(RewardType m_type)
    {
        switch (m_type)
        {
            case RewardType.Excitement:
                funEventSound?.Post(gameObject);
                break;
            case RewardType.Hunger:
                foodEventSound?.Post(gameObject);
                break;
            case RewardType.Coin:
                coinEventSound?.Post(gameObject);
                break;
            case RewardType.Paint:
                break;
            case RewardType.Care:
                careEventSound?.Post(gameObject);
                break;
            default:
                break;
        }
    }
}

public enum RewardType
{
    Care, Excitement, Hunger, Coin, Paint, XP
}
