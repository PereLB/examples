using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementAppearBehaviour", menuName = "UI/Reward/AppearBehaviour")]
public class ElementAppearJumpTweenBehaviour : ElementAppearBehaviourSOBase
{
    [SerializeField] private float jumpPower;
    [SerializeField] private int numJumps;
    [SerializeField] private Ease ease = Ease.Linear;
    private Tween tween;

    public override void Initialize(GameObject m_gameObject)
    {
        base.Initialize(m_gameObject);
    }

    public override float StartAppearAnimation(Vector3 m_targetPosition, int m_value)
    {
        Vector2 pos = gameObject.transform.position;
        pos.x = Screen.width*2.0f;
        gameObject.transform.position = pos;
        tween = gameObject.transform.DOJump(m_targetPosition, jumpPower, numJumps, (duration / 2.0f)).SetEase(ease);
        return duration;
    }

    public override void StopAppearAnimation(Vector3 m_targetPosition, int m_value) {
        if (tween!=null)
        {
            tween.Kill();
            tween = null;
        }
        gameObject.transform.position = m_targetPosition;
    }
}
