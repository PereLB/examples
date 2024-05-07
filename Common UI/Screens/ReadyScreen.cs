using UnityEngine;
using System;
using System.Collections;

public class ReadyScreen : Modal
{
    public Action OnStartGame;

    public void OnEnable()
    {
        StartCoroutine(ReadyGame_CRT());
    }

    private IEnumerator ReadyGame_CRT()
    {
        ReadyScreenAction[] extraActions = GetComponentsInChildren<ReadyScreenAction>();
        yield return new WaitForSeconds(0.1f);
        foreach (var extraAction in extraActions)
        {
            yield return extraAction.StartAction();
        }

        UIManager.Instance.ShowGroup("IngameScreen", true);
        OnStartGame?.Invoke();
    }
}
