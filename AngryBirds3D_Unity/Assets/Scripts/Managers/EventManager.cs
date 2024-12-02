using System;
using UnityEngine;

public static class EventManager
{
    public static Action OnBirdShot;
    public static Action OnBirdHit;

    public static Action OnPrepareGame;
    public static Action OnGameStart;

    public static void InvokeBirdShot()
    {
        OnBirdShot?.Invoke();
        Debug.Log("Invoked: BirdShot");
    }
    public static void InvokeBirdHit()
    {
        OnBirdHit?.Invoke();
        Debug.Log("Invoked: BirdHit");
    }

    public static void InvokePrepareGame()
    {
        OnPrepareGame?.Invoke();
        Debug.Log("Invoked: PrepareGame");
    }
    public static void InvokeGameStart()
    {
        OnGameStart?.Invoke();
        Debug.Log("Invoked: GameStart");
    }
}
