using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundInfoSummaryScreen : MonoBehaviour
{
    [Header("Listeners")]
    [SerializeField] private ElementChannelSO startUpdateElementEvent;
    [SerializeField] private ElementChannelSO endUpdateElementEvent;
    [SerializeField] private ExperienceChannelSO xpEvent;

    [Header("Possible Reward type")]
    [SerializeField] private List<RewardSO> typeRewards;

    [Space]
    [SerializeField] private List<GameObject> elements;

    private int statPointer;

    private void OnEnable()
    {
        statPointer = 0;
        if (xpEvent)
            xpEvent.OnPresentXP += UpdateXP;
        if (endUpdateElementEvent)
            endUpdateElementEvent.OnChangeElementState += UpdateElement;
    }

    private void OnDisable()
    {
        if (xpEvent)
            xpEvent.OnPresentXP -= UpdateXP;
        if (endUpdateElementEvent)
            endUpdateElementEvent.OnChangeElementState -= UpdateElement;
    }

    private void UpdateXP(int m_value)
    {
        elements[0].SetActive(true);
        elements[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+ " + m_value.ToString();
        statPointer++;
    }

    private void UpdateElement(RewardType m_rewardType, int m_value)
    {
        switch (m_rewardType)
        {
            case RewardType.Care:
                SetStatElement(m_value, GetSprite(m_rewardType));
                break;
            case RewardType.Excitement:
                SetStatElement(m_value, GetSprite(m_rewardType));
                break;
            case RewardType.Hunger:
                SetStatElement(m_value, GetSprite(m_rewardType));
                break;
            case RewardType.Coin:
                SetStatElement(m_value, GetSprite(m_rewardType));
                break;
            case RewardType.Paint:
                break;
            default:
                break;
        }
    }

    private void SetStatElement(int m_value, Sprite m_sprite)
    {
        elements[statPointer].SetActive(true);
        if (m_sprite != null)
        {
            elements[statPointer].transform.GetChild(0).gameObject.SetActive(true);
            elements[statPointer].transform.GetChild(0).GetComponent<Image>().sprite = m_sprite;
        }
        else
        {
            elements[statPointer].transform.GetChild(0).gameObject.SetActive(false);
        }
        elements[statPointer].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+ " + m_value.ToString();
        statPointer++;
    }

    private Sprite GetSprite(RewardType m_rewardType)
    {
        foreach (var reward in typeRewards)
        {
            if (reward.id == (int)m_rewardType)
            {
                return reward.rewardImage;
            }
        }
        return null;
    }
}
