using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "TRG/UI/Events/String Event Channel")]
public class StringEventChannelSO : ScriptableObject
{
    public UnityAction<string> OnEventRaised;

    public void RaiseEvent(string m_string)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(m_string);
    }
}
