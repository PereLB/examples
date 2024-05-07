using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class XPSummaryScreen : MonoBehaviour
{
    [SerializeField] private GameObject levelUpScreen;

    [SerializeField] private Image fillBar;
    [SerializeField] private TextMeshProUGUI xpTxt;

    [Header("Listener")]
    [SerializeField] private VoidEventChannelSO skipEvent;

    [Header("Broadcaster")]
    [SerializeField] private ExperienceChannelSO xpEvent;
    [SerializeField] private VoidEventChannelSO lvlupEvent;

    [Header("Audio Events")]
    [SerializeField] AK.Wwise.Event xpBarFillSound;
    [SerializeField] AK.Wwise.Event xpStingSound;

    [Space]
    [SerializeField] private float timeToFill=3.0f;
    private float tickToFill= 10;

    private Coroutine experience_CO;
    private Coroutine autoskip_CO;
    private Coroutine fillBar_CO;
    private Tween fillBar_Tween;
    private LevelData finalLevel;
    private float accumulatedFillBarExperience;
    private int new_currentExperience;

    private bool tapCooldown = false;
    private bool tapped = false;
    private bool done = false;

    private string defaultPet
    {
        get
        {
            return FirebaseManager.Instance.UserData.activePet;
        }
    }

    private List<PetMetadata> ownedPets
    {
        get
        {
            return FirebaseManager.Instance.ownedPetData;
        }
    }


    private void SubscribeInput()
    {
        if (skipEvent)
            skipEvent.OnEventRaised += SkipButton;
        if (InputManager.Instance)
        {
            InputManager.Instance.OnTap += Tap;
        }
    }

    private void UnsubscribeInput()
    {
        if (skipEvent)
            skipEvent.OnEventRaised -= SkipButton;
        if (InputManager.Instance)
        {
            InputManager.Instance.OnTap -= Tap;
        }
    }

    public void PopulateScreen(Action m_callback, int m_xpCurrent, int m_xpEarned, int m_xpTNL, int m_currentLevel)
    {
        experience_CO = StartCoroutine(AddExperience(m_callback, m_xpCurrent, m_xpEarned, m_xpTNL, m_currentLevel));
    }

    private IEnumerator AddExperience(Action m_callback, int m_xpCurrent, int m_xpEarned, int m_xpTNL, int m_currentLevel)
    {
        accumulatedFillBarExperience = 0;
        fillBar_CO= StartCoroutine(FillBar(m_xpCurrent, m_xpEarned, m_xpTNL, m_currentLevel));
        SubscribeInput();
        yield return new WaitUntil(() => tapped || done);
        StopCoroutine(fillBar_CO);
        fillBar_Tween?.Kill();
        UnsubscribeInput();
        if (xpEvent)
            xpEvent.RaiseEvent(m_xpEarned);
        m_callback?.Invoke();
    }

    private IEnumerator FillBar(int m_xpCurrent, int m_xpEarned, int m_xpTNL, int m_currentLevel)
    {
        xpBarFillSound?.Post(gameObject);
        xpStingSound?.Post(gameObject);

        xpTxt.text = "+ "+ accumulatedFillBarExperience.ToString("N0");
        float xpEarned = Mathf.Clamp(m_xpEarned, 0, m_xpTNL);
        if (m_xpCurrent + m_xpEarned > m_xpTNL && m_xpCurrent>0)
            xpEarned = m_xpEarned - ((m_xpCurrent + m_xpEarned) - m_xpTNL);

        fillBar.fillAmount = ((float)m_xpCurrent / (float)m_xpTNL);
        fillBar_Tween = fillBar.DOFillAmount(((float)m_xpCurrent + (float)xpEarned) / (float)m_xpTNL, timeToFill);

        float xpTick = (float)xpEarned / tickToFill;
        float timeTick =  timeToFill/ tickToFill;
        int tick = 1;
        while (tick <= tickToFill)
        {
            accumulatedFillBarExperience += xpTick;
            xpTxt.text ="+ "+ ((float)accumulatedFillBarExperience).ToString("N0");
            tick++;
            yield return new WaitForSeconds(timeTick);
        }

        //User level ups
        if (m_xpCurrent + m_xpEarned >= m_xpTNL)
        {
            ActivateLevelUP();
            int new_xpEarned = ((m_xpCurrent + m_xpEarned) - m_xpTNL);
            finalLevel = FirebaseManager.Instance?.GetNextLevelData(m_currentLevel);
            if (finalLevel != null)
                fillBar_CO = StartCoroutine(FillBar(0, new_xpEarned, finalLevel.nextLevel, finalLevel.level));
            else
                done = true;
        }
        else
        {
            done = true;
        }
    }

    private void SkipFillBar(int m_xpCurrent, int m_xpEarned, int m_xpTNL)
    {
        StopCoroutine(fillBar_CO);
        fillBar_Tween?.Kill();
        if (m_xpCurrent + m_xpEarned >= m_xpTNL)
        {
            ActivateLevelUP();

            finalLevel = FirebaseManager.Instance.GetNextLevelData(FirebaseManager.Instance.GetLevelData(m_xpCurrent).level);
            new_currentExperience = ((m_xpCurrent + m_xpEarned) - m_xpTNL);
            fillBar.fillAmount = ((float)new_currentExperience) / (float)finalLevel.nextLevel;
            xpTxt.text = "+ " + (new_currentExperience).ToString("N0");
        }
        else {
            fillBar.fillAmount = ((float)m_xpCurrent + (float)m_xpEarned) / (float)m_xpTNL;
            xpTxt.text = "+ " + (m_xpEarned).ToString("N0");
        }
    }

    private void ActivateLevelUP()
    {
        if (lvlupEvent)
            lvlupEvent.RaiseEvent();
        levelUpScreen.SetActive(false);
        levelUpScreen.SetActive(true);
        fillBar.transform.parent.DOShakeRotation(0.4f, 5, 7);
        fillBar.transform.parent.DOShakePosition(0.4f, new Vector3(0.75f, 0.0f, 0.0f), 10, 0, false, false, ShakeRandomnessMode.Harmonic);
    }

    private void LevelUp(Action m_callback, int m_xpEarned)
    {
        levelUpScreen.SetActive(true);
        levelUpScreen.GetComponent<LevelUpScreen>().SpawnOptions(m_callback, m_xpEarned);
    }

    public void SkipButton()
    {
        UnsubscribeInput();
        if (PetUpdateEventManager.Instance)
        {
            AnalyticManager.TriggerEvent(AnalyticDefinitions.MinigameSummary(PetUpdateEventManager.Instance.m_activeMinigame, true), true, false);
        }
        SceneManager.Instance.LoadHub();
    }

    private void Tap(Vector3 m_pos)
    {
        if (tapCooldown) return;
        tapCooldown = true;
        tapped = true;
    }

    private IEnumerator Autoskip()
    {
        yield return new WaitForSeconds(6.0f);
        Tap(Vector3.zero);
    }
}
