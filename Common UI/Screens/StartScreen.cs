using System;

public class StartScreen : Modal
{
    public Action<bool> OnExit;

    public void ReadyGame()
    {
        if (PetUpdateEventManager.Instance)
        {
            AnalyticManager.TriggerEvent(AnalyticDefinitions.MinigameStart(PetUpdateEventManager.Instance.m_activeMinigame), true, false);
        }
        UIManager.Instance.ShowGroup("ReadyScreen", true);
    }

    public void Exit()
    {
        if (PetUpdateEventManager.Instance)
        {
            AnalyticManager.TriggerEvent(AnalyticDefinitions.MinigameQuit(PetUpdateEventManager.Instance.m_activeMinigame, true), true, false);
        }
        OnExit?.Invoke(true);
    }
}
