using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Environment Tint")]
    [SerializeField] private Color _environmentTint = new(123, 123, 123, 255);
    [SerializeField] private Material _material;
    [SerializeField] private float _evnironmentTintTransitionDuration = 2.0f;
    [SerializeField] private float _preperationDuration = 0.5f;
    [SerializeField] private float _startGameCamDelay = 0.5f;

    [Header("Camera Transitions")]
    [SerializeField] private CameraController _camController;

    

    private void OnApplicationQuit()
    {
        _material.color = Color.white;
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
    private IEnumerator StartGameRoutine()
    {
        // first transition
        _camController.SetTransitionCamera();
        yield return StartCoroutine(ApplyTintOnEnvironmentRoutine());

        EventManager.InvokePrepareGame();
        yield return new WaitForSeconds(_preperationDuration);

        // second transition
        _camController.SetNewFollowBird();
        yield return new WaitForSeconds(_startGameCamDelay);

        _camController.SetGameCamera();
        EventManager.InvokeGameStart();
    }

    [ContextMenu("Start Game")]
    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }
}
