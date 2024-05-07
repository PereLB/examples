using UnityEngine;
using DG.Tweening;

public class SummaryCatReaction : MonoBehaviour
{
    private float skilledThreshold = 30.0f;
    private ElementChannelSO startUpdateElementEvent;
    private ElementChannelSO endUpdateElementEvent;
    private VoidEventChannelSO lvlupEvent;

    private Tween animTween;
    private Vector3 startPosition;
    private Animator anim;
    private Camera targetCamera;

    public void Initialize(ElementChannelSO m_startUpdate, ElementChannelSO m_endUpdate, VoidEventChannelSO m_lvlupEvent, Vector3 m_pos,Camera _target = null)
    {
        startUpdateElementEvent = m_startUpdate;
        endUpdateElementEvent = m_endUpdate;
        lvlupEvent = m_lvlupEvent;
        startPosition = m_pos;

        targetCamera = _target ? _target : Camera.main;

        anim = GetComponent<Animator>();
        EntryAnimation(startPosition);
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        if (startUpdateElementEvent)
            startUpdateElementEvent.OnChangeElementState += StartElement;
        if (endUpdateElementEvent)
            endUpdateElementEvent.OnChangeElementState += EndElement;
        if (lvlupEvent)
            lvlupEvent.OnEventRaised += LevelUp;
    }

    private void UnsusbcribeEvents()
    {
        if (startUpdateElementEvent)
            startUpdateElementEvent.OnChangeElementState -= StartElement;
        if (endUpdateElementEvent)
            endUpdateElementEvent.OnChangeElementState -= EndElement;
        if (lvlupEvent)
            lvlupEvent.OnEventRaised -= LevelUp;
    }

    private void OnDisable()
    {
        UnsusbcribeEvents();
    }
    [ContextMenu("EntryAnimation")]
    private void EntryAnimation(Vector3 m_pos)
    {
        Plane camerPlane = new Plane(targetCamera.transform.forward, targetCamera.transform.position);
        this.transform.LookAt(camerPlane.ClosestPointOnPlane(this.transform.position));
        anim.SetTrigger("JumpLeft");
        this.transform.DOMove(startPosition, 0.833f);
        Transform oldParent = this.transform.parent;
        this.transform.SetParent(targetCamera.transform);
        Vector3 rot = gameObject.transform.localEulerAngles;
        rot += new Vector3(-5.0f, 15.0f, 0.0f);
        this.transform.DOLocalRotate(rot, 0.833f).OnComplete(() =>
        {
            Quaternion qrot = gameObject.transform.rotation;
            this.transform.SetParent(oldParent);
            this.transform.rotation = qrot;
        });

        this.gameObject.GetComponent<HeadEyeLook>().lookTargetObject = targetCamera.transform;
        this.transform.GetComponentInChildren<Canvas>().worldCamera = targetCamera;
    }

    private void StartElement(RewardType m_rewardType, int m_value)
    {
        switch (m_rewardType)
        {
            case RewardType.Care:
                if (m_value >= skilledThreshold)
                    anim.SetTrigger("CareSkilled");
                else
                    anim.SetTrigger("CareNormal");
                break;
            case RewardType.Excitement:
                if (m_value >= skilledThreshold)
                    anim.SetTrigger("FunSkilled");
                else
                    anim.SetTrigger("Punching");
                break;
            case RewardType.Hunger:
                if (m_value >= skilledThreshold)
                    anim.SetTrigger("EatSkilled");
                else
                    anim.SetTrigger("EatNormal");
                break;
            case RewardType.Coin:
                anim.SetTrigger("Punching");
                break;
            case RewardType.Paint:
                break;
            case RewardType.XP:
                break;
            default:
                break;
        }
    }

    private void LevelUp()
    {
        anim.SetTrigger("Punching");
    }

    private void EndElement(RewardType m_rewardType, int m_value)
    {
        if (animTween != null)
        {
            animTween.Kill();
            animTween = null;
            if(startPosition!=null)
                this.transform.position = startPosition;
        }
    }
}
