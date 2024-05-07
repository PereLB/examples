using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "UI/Events/Float Event Channel")]
public class FloatEventChannelSO : ScriptableObject
{
    public UnityAction<float> OnEventRaised;

    public void RaiseEvent(float m_float)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(m_float);
    }
}
