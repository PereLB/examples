using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "UI/Events/Element Event Channel")]
public class ElementChannelSO : ScriptableObject
{
    public UnityAction<RewardType, int> OnChangeElementState;

    public void RaiseEvent(RewardType m_rewardType, int m_amount)
    {
        if (OnChangeElementState != null) { 
            OnChangeElementState.Invoke(m_rewardType, m_amount);
        }
        else
        {
            Debug.LogWarning("A ChangeElementState was raised, but no one was listening :(");
        }
    }
}