using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using System.Collections;

public class LevelUpScreen : Modal
{
    #region Variables

    [SerializeField] private GameObject optionContainer;
    [SerializeField] private GameObject optionPrefab;
    [SerializeField] private List<UIParticleSystem> particles;
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private float ScaleTweenDuration;

    [Space]
    [Header("Broadcaster")]
    [SerializeField] private ExperienceChannelSO xpEvent;

    private List<Vector3> OGScales;
    private bool tapCooldown = false;
    private bool tapped = false;

    private Coroutine levelUp_CO;
    private Coroutine autoskip_CO;

    #endregion

    #region Lifecycle

    private void OnEnable()
    {
        OGScales = new List<Vector3>();
        Vector3 OGScale;

        foreach (var particle in particles)
        {
            OGScale = particle.gameObject.transform.localScale;
            OGScales.Add(OGScale);
            particle.gameObject.transform.localScale = Vector3.zero;
            particle.gameObject.transform.DOScale(OGScales[OGScales.Count-1], ScaleTweenDuration);
        }

        OGScale = levelUpText.gameObject.transform.localScale;
        OGScales.Add(OGScale);
        levelUpText.gameObject.transform.localScale = Vector3.zero;
        levelUpText.transform.DOScale(Vector3.one, ScaleTweenDuration);
    }

    #endregion

    #region Input subscription

    private void SubscribeInput()
    {
        if (InputManager.Instance)
        {
            InputManager.Instance.OnTap += Tap;
        }
    }

    private void UnsubscribeInput()
    {
        if (InputManager.Instance)
        {
            InputManager.Instance.OnTap -= Tap;
        }
    }

    #endregion

    public void SpawnOptions(Action m_callback, int m_xpEarned)
    {
        levelUp_CO = StartCoroutine(WaitForUserInput(m_callback, m_xpEarned));
        autoskip_CO = StartCoroutine(Autoskip());
    }

    private IEnumerator WaitForUserInput(Action m_callback, int m_xpEarned)
    {
        SubscribeInput();
        yield return new WaitUntil(() => tapped);
        if (xpEvent)
            xpEvent.RaiseEvent(m_xpEarned);
        m_callback?.Invoke();
    }

    private IEnumerator Autoskip()
    {
        yield return new WaitForSeconds(4.0f);
        Tap(Vector3.zero);
    }

    private void Tap(Vector3 m_pos)
    {
        if (tapCooldown) return;
        tapCooldown = true;
        tapped = true;
        if (autoskip_CO != null)
            StopCoroutine(autoskip_CO);
        UnsubscribeInput();
    }
}
