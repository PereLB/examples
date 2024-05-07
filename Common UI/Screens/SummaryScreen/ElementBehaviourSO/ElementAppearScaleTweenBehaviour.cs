using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ElementAppearBehaviour", menuName = "UI/Reward/AppearScaleBehaviour")]
public class ElementAppearScaleTweenBehaviour : ElementAppearBehaviourSOBase
{
    [SerializeField] private Vector3 scale;
    [SerializeField] private Ease ease = Ease.Linear;
    [SerializeField] private Ease secondEase = Ease.Linear;
    [SerializeField] private GameObject particleSystem;
    private Tween tween;
    private Tween secondTween;

    private Vector3 startScale;
    private Vector3 secondStartScale;
    private Vector3 endPosition;

    private GameObject particleSystemContainer;
    private TextMeshProUGUI nameElement;
    private TextMeshProUGUI amountElement;
    private Image imageElement;

    public override void Initialize(GameObject m_gameObject)
    {
        base.Initialize(m_gameObject);
        startScale = gameObject.transform.localScale;
        particleSystemContainer = gameObject.transform.GetChild(0).gameObject;
        nameElement = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        amountElement = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        imageElement = gameObject.transform.GetChild(3).GetComponent<Image>();
        gameObject.transform.localPosition = new Vector3(0, gameObject.GetComponent<RectTransform>().rect.height / 2.0f, 0);
        secondStartScale = amountElement.transform.localScale;
        endPosition = gameObject.transform.position;
    }

    public override float StartAppearAnimation(Vector3 m_targetPosition, int m_value)
    {
        endPosition = m_targetPosition;
        UIParticleSystem ps = Instantiate(particleSystem, particleSystemContainer.transform).GetComponent<UIParticleSystem>();
        ps.gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localScale = Vector3.zero;
        amountElement.transform.localScale = Vector3.zero;
        tween = gameObject.transform.DOScale(scale, (1.0f * duration / 3.0f)).SetEase(ease).OnComplete(()=>{
            secondTween = amountElement.transform.DOScale(Vector3.one, (1.0f * duration / 3.0f)).SetEase(secondEase).OnComplete(()=> {
                if(m_targetPosition.y>= gameObject.transform.position.y)
                    gameObject.transform.DOMove(m_targetPosition, (1.0f * duration / 3.0f));
            });
        });
        return duration;
    }

    public override void StopAppearAnimation(Vector3 m_targetPosition, int m_value) {
        if (tween!=null)
        {
            tween.Kill();
            tween = null;
        }
        if (secondTween != null)
        {
            secondTween.Kill();
            secondTween = null;
        }
        gameObject.transform.localScale = startScale;
        gameObject.transform.position = endPosition;
        amountElement.transform.localScale = secondStartScale;
    }
}
