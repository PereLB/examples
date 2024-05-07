using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "UI/Events/Experience Event Channel")]
public class ExperienceChannelSO : ScriptableObject
{
    public UnityAction<int> OnPresentXP;

    public void RaiseEvent(int m_amount)
    {
        if (OnPresentXP != null)
        {
            OnPresentXP.Invoke(m_amount);
        }
        else
        {
            Debug.LogWarning("A PresentXP was requested, but no one was listening :(");
        }
    }
}