using UnityEngine;
using UnityEngine.AI;

public class Pig : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private LayerMask _walkableLayer, _birdLayer;
    [SerializeField] private float _runAwaySpeed = 4.0f;
    private Bird _detectedBird = null;

    [Header("Patrol")]
    [SerializeField] private float _distanceToNextPoint;
    [SerializeField] private Vector3 _nextPoint;
    [SerializeField] private float _patrolSpeed = 3.0f;
    private bool _isNextPointSet = false;

    [Header("States")]
    [SerializeField] private float _detectBirdDistance = 15.0f;
    [SerializeField] private float _runAwayDistance = 5.0f;
    private bool _isBirdDetected = false;

    private delegate void State();
    private State _state;

    private void Start()
    {
        _agent.speed = _patrolSpeed;
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

    #region States
    private void PatrollingState()
    {
        if (!_isNextPointSet) LookForNextPoint();
        if (_isNextPointSet) _agent.SetDestination(_nextPoint);

        Vector3 distanceToTravel = transform.position - _nextPoint;
        if (distanceToTravel.magnitude < 1.0f)
        {
            _isNextPointSet = false;
        }

        if (_detectedBird) // if bird exist (should be attached automatically)
        {
            float distanceFromBird = Vector3.Distance(_detectedBird.transform.position, transform.position);
            if (_detectBirdDistance < distanceFromBird)
            {
                _isBirdDetected = true;
                _state = InShockState; // state change
                return;
            }
        }
    }
    private void InShockState()
    {
        if (_detectedBird && _isBirdDetected) // if bird exist (should be attached automatically), and detected
        {
            float distanceFromBird = Vector3.Distance(_detectedBird.transform.position, transform.position);
            if (_runAwayDistance < distanceFromBird)
            {
                _agent.speed = _runAwaySpeed;
                _state = RunAwayState; // state change
                return;
            }
        }
    }
    private void RunAwayState()
    {
        if (_detectedBird) // if bird exist (should be attached automatically)
        {
            float distanceFromBird = Vector3.Distance(_detectedBird.transform.position, transform.position);
            if (distanceFromBird > _runAwayDistance)
            {
                _agent.speed = _patrolSpeed;
                _state = PatrollingState; // state change
                return;
            }

            Vector3 birdDirection = (_detectedBird.transform.position - transform.position).normalized;
            if (distanceFromBird <= _runAwayDistance)
            {
                Vector3 runAwayDestination = -birdDirection.normalized * _runAwaySpeed;
                _agent.SetDestination(transform.position + runAwayDestination);
            }
            
            return;
        }

        _agent.speed = _patrolSpeed;
        _state = PatrollingState;
    }
    #endregion

    private void LookForNextPoint()
    {
        float x = Random.Range(-_distanceToNextPoint, _distanceToNextPoint);
        float z = Random.Range(-_distanceToNextPoint, _distanceToNextPoint);

        _nextPoint = new(transform.position.x + x, transform.position.y, transform.position.z + z);
        if (Physics.Raycast(_nextPoint, -transform.up, 2.0f, _walkableLayer))
        {
            _isNextPointSet = true;
        }
    }

    private void OnBirdSpawned(Bird bird)
    {
        _detectedBird = bird;
    }
}
