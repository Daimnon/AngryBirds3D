using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    private static AnalyticsManager _instance = null;
    public static AnalyticsManager Instance => _instance;

    private bool _isInitialized = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;
    }
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StopDataCollection();
        _isInitialized = true;
    }

    public void UpdateScore(int newScore)
    {
        if (!_isInitialized) return;

        CustomEvent scoreEvent = new("score_track")
        {
            {"score",  newScore}
        };
        AnalyticsService.Instance.RecordEvent(scoreEvent);
        AnalyticsService.Instance.Flush();
    }
}
