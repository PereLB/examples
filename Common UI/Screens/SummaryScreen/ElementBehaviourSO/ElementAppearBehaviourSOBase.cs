using UnityEngine;

public class ElementAppearBehaviourSOBase : ScriptableObject
{
    protected GameObject gameObject;
    [SerializeField] protected float duration;

    public virtual void Initialize(GameObject m_gameObject)
    {
        this.gameObject = m_gameObject;
        if (gameObject.transform.GetChild(0).childCount > 0)
        {
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<UIParticleSystem>().StopParticleEmission();
            Destroy(gameObject.transform.GetChild(0).GetChild(0).gameObject);
        }
    }

    public virtual float StartAppearAnimation(Vector3 m_targetPosition, int m_value)
    {
        return duration;
    }

    public virtual void StopAppearAnimation(Vector3 m_targetPosition, int m_value) {}
}
