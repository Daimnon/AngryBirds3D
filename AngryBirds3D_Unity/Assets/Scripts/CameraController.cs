using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _gameCamera;
    [SerializeField] private float _startDelay = 2.0f; // same as camera blend time

    private void Start()
    {
        StartCoroutine(FollowBirdRoutine());
    }

    private IEnumerator FollowBirdRoutine()
    {
        BirdManager bManager = BirdManager.Instance;
        yield return new WaitForSeconds(_startDelay);

        if (bManager.TryGetComponent(out Bird readyBird))
        {
            _gameCamera.Target.TrackingTarget = readyBird.transform;
        }
    }
}
