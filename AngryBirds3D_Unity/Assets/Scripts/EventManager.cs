using System;
using UnityEngine;

public static class EventManager
{
    public static Action OnBirdShot;

    public static void InvokeBirdShot()
    {
        OnBirdShot?.Invoke();
        Debug.Log("Invoked: BirdShot");
    }
}
