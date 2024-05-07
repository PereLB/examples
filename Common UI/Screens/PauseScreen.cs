using UnityEngine;
using System;

public class PauseScreen : Modal
{
    #region Variables

    public Action OnResume;
    public Action<bool> OnEnd;
    [SerializeField] private AK.Wwise.State pauseState;
    [SerializeField] private AK.Wwise.State inGameState;

    #endregion

    #region Lifecycle

    private void OnEnable()
    {
        pauseState.SetValue();
    }

    private void OnDisable()
    {
        inGameState.SetValue();
    }

    #endregion

    public void Resume()
    {
        UIManager.Instance.HideGroup("PauseScreen");
        OnResume?.Invoke();
    }

    public void End()
    {
        if (PetUpdateEventManager.Instance)
        {
            AnalyticManager.TriggerEvent(AnalyticDefinitions.MinigameQuit(PetUpdateEventManager.Instance.m_activeMinigame, false), true, false);
        }
        OnEnd?.Invoke(true);
    }
}
