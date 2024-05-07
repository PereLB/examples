using UnityEngine;
using System;

public abstract class ReadyScreenAction : MonoBehaviour
{
    public abstract Coroutine StartAction(Action successCallback = null, Action<string> failCallback = null);
}