using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _entryCam;
    public CinemachineCamera EntryCam => _entryCam;

    [SerializeField] private CinemachineCamera _transitionCam;
    public CinemachineCamera TransitionCam => _transitionCam;

    [SerializeField] private CinemachineCamera _gameCam;
    public CinemachineCamera GameCam => _gameCam;

    public void SetTransitionCamera()
    {
        _entryCam.Priority = 0;
        _transitionCam.Priority = 2;
        _gameCam.Priority = 1;
    }
    public void SetGameCamera()
    {
        _entryCam.Priority = 0;
        _transitionCam.Priority = 1;
        _gameCam.Priority = 2;
    }

    private IEnumerator FollowBirdRoutine()
    {
        BirdManager bManager = BirdManager.Instance;
        while (!bManager.ReadyBird)
        {
            yield return null;
        }
        _gameCam.Target.TrackingTarget = bManager.ReadyBird.transform;
    }
    /// <summary>
    /// Start a courotine which will attemp to get the ReadyBird
    /// from the BirdManager until it succeded.
    /// </summary>
    public void SetNewFollowBird()
    {
        StartCoroutine(FollowBirdRoutine());
    }
}
