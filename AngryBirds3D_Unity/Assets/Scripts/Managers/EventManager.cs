using System;
using UnityEngine;

public static class EventManager
{
    public static Action OnBirdShot;
    public static Action OnBirdHit;

    public static void InvokeBirdShot()
    {
        OnBirdShot?.Invoke();
        Debug.Log("Invoked: BirdShot");
    }
    public static void InvokeBirdHit(Bird bird)
    {
        if (!bird)
        {
            Debug.Log("Invoke failed: BirdHit, No bird was passed");
            return;
        }

        OnBirdHit?.Invoke();
        Debug.Log("Invoked: BirdHit");
    }
}
