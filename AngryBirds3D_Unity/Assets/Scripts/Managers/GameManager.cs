using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Environment Tint")]
    [SerializeField] private Color _environmentTint = new(123, 123, 123, 255);
    [SerializeField] private Material _material;
    [SerializeField] private float _evnironmentTintTransitionDuration = 2.0f;

    [Header("Camera Transitions")]
    [SerializeField] private CinemachineCamera _entryCam;
    [SerializeField] private CinemachineCamera _gameCam;

    private void Start()
    {
        _entryCam.Priority = 0;
        _gameCam.Priority = 1;
        StartCoroutine(ApplyTintOnEnvironmentRoutine());
    }

    private IEnumerator ApplyTintOnEnvironmentRoutine()
    {
        Color initialColor = _material.color;
        float elapsedTime = 0f;

        while (elapsedTime < _evnironmentTintTransitionDuration)
        {
            float t = elapsedTime / _evnironmentTintTransitionDuration;
            _material.color = Color.Lerp(initialColor, _environmentTint, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _material.color = _environmentTint;
    }

    private void OnApplicationQuit()
    {
        _material.color = Color.white;
    }
}
