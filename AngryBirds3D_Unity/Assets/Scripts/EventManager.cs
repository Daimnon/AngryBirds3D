using System;
using UnityEngine;

public static class EventManager
{
    // might not need <Bird>
    public static Action<Bird> OnBirdShot;
    public static Action<Bird> OnBirdHit;
    // ---------------------


    public static void InvokeBirdShot(Bird bird)
    {
        if (!bird)
        {
            Debug.Log("Invoke failed: BirdShot, No bird was passed");
            return;
        }

        OnBirdShot?.Invoke(bird);
        Debug.Log("Invoked: BirdShot");
    }
    public static void InvokeBirdHit(Bird bird)
    {
        if (!bird)
        {
            Debug.Log("Invoke failed: BirdHit, No bird was passed");
            return;
        }

        OnBirdHit?.Invoke(bird);
        Debug.Log("Invoked: BirdHit");
    }
}
