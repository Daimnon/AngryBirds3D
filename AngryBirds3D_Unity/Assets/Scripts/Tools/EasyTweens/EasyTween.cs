using System.Collections;
using UnityEngine;

public static class EasyTween
{
    /* recommanded starting values
     _startValue = 0.0f;
     _endValue = 10.0f;
     _duration = 2.0f; */

    public static IEnumerator EaseIn(float lerpValue, float endValue, float duration)
    {
        float elapsed = 0f;
        float startValue = lerpValue;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * t; // Ease In: cubic function
            lerpValue = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }
        lerpValue = endValue;
    }
    public static IEnumerator EaseOut(float lerpValue, float endValue, float duration)
    {
        float elapsed = 0f;
        float startValue = lerpValue;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = 1 - Mathf.Pow(1 - t, 3); // Ease Out: cubic function
            lerpValue = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }
        lerpValue = endValue;
    }
    public static IEnumerator EaseInOut(float lerpValue, float endValue, float duration)
    {
        float elapsed = 0f;
        float startValue = lerpValue;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2; // Ease In-Out
            lerpValue = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }
        lerpValue = endValue;
    }
    public static IEnumerator Elastic(float lerpValue, float endValue, float duration)
    {
        float elapsed = 0f;
        float startValue = lerpValue;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.Sin(13 * Mathf.PI / 2 * t) * Mathf.Pow(2, 10 * (t - 1)); // Elastic effect
            lerpValue = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }
        lerpValue = endValue;
    }
    public static IEnumerator Bounce(float lerpValue, float endValue, float duration)
    {
        float elapsed = 0f;
        float startValue = lerpValue;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (t < 1 / 2.75f)
                t = 7.5625f * t * t;
            else if (t < 2 / 2.75f)
            {
                t -= 1.5f / 2.75f;
                t = 7.5625f * t * t + 0.75f;
            }
            else if (t < 2.5f / 2.75f)
            {
                t -= 2.25f / 2.75f;
                t = 7.5625f * t * t + 0.9375f;
            }
            else
            {
                t -= 2.625f / 2.75f;
                t = 7.5625f * t * t + 0.984375f;
            }

            lerpValue = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }
        lerpValue = endValue;
    }
}
