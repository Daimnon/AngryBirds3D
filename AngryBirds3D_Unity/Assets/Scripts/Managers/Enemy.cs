using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private LayerMask _walkableLayer, _birdLayer;
    private Bird _detectedBird = null;

    [Header("Patrol")]
    [SerializeField] private float _distanceToNextPoint;
    [SerializeField] private Vector3 _nextPoint;
    private bool _isNextPointSet = false;

    [Header("States")]
    [SerializeField] private float _detectBirdDistance = 15.0f;
    [SerializeField] private float _runAwayDistance = 5.0f;
    private bool _isBirdDetected = false;

    private delegate void State();
    private State _state;

    private void Start()
    {
        _state = PatrollingState;
    }
    private void OnEnable()
    {
        EventManager.OnBirdSpawned += OnBirdSpawned;
    }
    private void OnDisable()
    {
        EventManager.OnBirdSpawned -= OnBirdSpawned;
    }
    private void Update()
    {
        _state.Invoke();
    }

    private void PatrollingState()
    {
        if (_detectedBird)
        {
            float distanceFromBird = Vector3.Distance(_detectedBird.transform.position, transform.position);
            if (_detectBirdDistance < distanceFromBird)
            {
                _isBirdDetected = true;
                _state = InShockState; // state change
            }
        }
    }
    private void InShockState()
    {
        if (_detectedBird && _isBirdDetected)
        {
            float distanceFromBird = Vector3.Distance(_detectedBird.transform.position, transform.position);
            if (_runAwayDistance < distanceFromBird)
            {
                _state = RunAwayState; // state change
            }
        }
    }
    private void RunAwayState()
    {
        if (_detectedBird)
        {
            float distanceFromBird = Vector3.Distance(_detectedBird.transform.position, transform.position);
            if (distanceFromBird > _detectBirdDistance)
            {
                _state = PatrollingState; // state change
            }
        }
    }
    private void OnBirdSpawned(Bird bird)
    {
        _detectedBird = bird;
    }
}
