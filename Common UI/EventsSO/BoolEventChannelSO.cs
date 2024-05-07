using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "UI/Events/Bool Event Channel")]
public class BoolEventChannelSO : ScriptableObject
{
    public UnityAction<bool> OnEventRaised;

    public void RaiseEvent(bool m_bool)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(m_bool);
    }
}
