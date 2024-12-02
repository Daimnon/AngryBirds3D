using System;
using UnityEngine;

public static class EventManager
{
    public static Action OnBirdShot;
    public static Action OnBirdHit;

    public static Action OnPrepareGame;
    public static Action OnGameStart;

    public static Action OnGameOver;

    public static void InvokeBirdShot()
    {
        OnBirdShot?.Invoke();
        Debugger.Log("Invoked: BirdShot");
    }
    public static void InvokeBirdHit()
    {
        OnBirdHit?.Invoke();
        Debugger.Log("Invoked: BirdHit");
    }

    public static void InvokePrepareGame()
    {
        OnPrepareGame?.Invoke();
        Debugger.Log("Invoked: PrepareGame");
    }
    public static void InvokeGameStart()
    {
        OnGameStart?.Invoke();
        Debugger.Log("Invoked: GameStart");
    }

    public static void InvokeGameOver()
    {
        OnGameOver?.Invoke();
        Debugger.Log("Invoked: GameOver");
    }
}
