using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird : MonoBehaviour
{
    private InputSystem_Actions _controls;
    private InputAction _interactAction;
    private Camera _mainCam;

    #region Properties
    [SerializeField] private BirdType _birdType;
    public BirdType @BirdType => _birdType;
    
    [SerializeField] private GameObject _anchorGO; // anchor is recieved when spawned form BirdManager
    public GameObject AnchorGO { get => _anchorGO; set => _anchorGO = value; }

    [SerializeField] private SpringJoint _anchorJoint; // joint connectedBody is recieved when spawned form BirdManager
    public SpringJoint AnchorJoint { get => _anchorJoint; set => _anchorJoint = value; }

    [SerializeField] private Rigidbody _rb;
    public Rigidbody Rb => _rb;

    private BirdManager _birdManager;
    public BirdManager BirdManager { get => _birdManager; set => _birdManager = value; }
    #endregion

    #region Serialized Members
    [Header("Config")]
    [SerializeField, Range(0.1f, 10.0f)] private float _releaseDistanceFromAnchor = 0.75f;
    [SerializeField] private float _dragDistanceFromAnchor = 2.0f;
    [SerializeField] private float _timeToDie = 2.0f;
    [SerializeField] private LayerMask _interactableLayer;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _spawnSounds;
    [SerializeField] private AudioClip[] _shotAudioClips;
    [SerializeField] private AudioClip[] _impactAudioClips;

    [Header("VFX & Animation")]
    [SerializeField] private Animator[] _animators;
    [SerializeField] private ParticleSystem _feathersImpactVFX;
    #endregion

    #region Members
    private Mouse _pointer;
    private Vector3 _pointerPos = Vector2.zero;
    private float _deathTimer = 0;
    private bool _wasShot = false; // if has been shot already
    private bool _isHit = false;
    #endregion

    #region Monobehaviour Callbacks
    private void OnEnable()
    {
        _wasShot = false;
        _isHit = false;
        _rb.angularVelocity = Vector3.zero;

        EnableInputs();
        PlaySpawnSound();
    }
    private void OnDisable()
    {
        DisableInputs();
        _wasShot = false;
        _isHit = false;
    }
    private void Awake()
    {
        _controls = new();
        InitializeInputs();
    }
    private void Start()
    {
        _mainCam = Camera.main;
        _pointer = Mouse.current;
        _pointerPos = new Vector3(_pointer.position.value.x, _pointer.position.value.y, _mainCam.nearClipPlane);
    }
    private void Update()
    {
        _pointerPos = _pointer.position.value;
        _pointerPos.z = _mainCam.nearClipPlane;

        if (_isHit)
        {
            _deathTimer -= Time.deltaTime;
            if (_deathTimer <= 0)
            {
                _birdManager.ReturnBirdToPool(this);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Hit(collision);
    }
    #endregion

    #region Inputs Handling
    private void InitializeInputs()
    {
        _interactAction = _controls.Player.Interact;
    }
    private void EnableInputs()
    {
        _interactAction.Enable();

        _controls.Player.Interact.started += OnPointerClick;
        _controls.Player.Interact.canceled += OnPointerRelease;
    }
    private void DisableInputs()
    {
        _controls.Player.Interact.started -= OnPointerClick;
        _controls.Player.Interact.canceled -= OnPointerRelease;

        _interactAction.Disable();
    }
    #endregion

    #region Inputs Logic
    private void OnPointerClick(InputAction.CallbackContext context)
    {
        if (_wasShot) return;

        float distance = Mathf.Abs(_mainCam.transform.position.z) + 1;
        Vector3 origin = _mainCam.ScreenToWorldPoint(_pointerPos);
        Vector3 direction = (origin - _mainCam.transform.position).normalized;

        Ray ray = new(origin, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, _interactableLayer))
        {
            StartAiming();
        }
    }
    private void OnPointerRelease(InputAction.CallbackContext context)
    {
        if (_wasShot) return;

        StopAiming();
    }
    #endregion

    #region Behaviours
    private IEnumerator AimRoutine()
    {
        if (!_anchorJoint)
        {
            Debugger.Log("No anchor connected to bird");
            yield break;
        }

        // every frame checks if mouse btn was let go
        while (!_interactAction.WasReleasedThisFrame())
        {
            Vector3 targetPos = _pointerPos;
            float zDepth = Mathf.Abs(_mainCam.transform.position.z - transform.position.z);

            targetPos.z = zDepth;
            targetPos = _mainCam.ScreenToWorldPoint(targetPos);

            bool isWithinDragDistance = Vector3.Distance(targetPos, _anchorJoint.connectedBody.position) > _dragDistanceFromAnchor;
            Vector3 clampedTargetPos = _anchorJoint.connectedBody.position + (targetPos - _anchorJoint.connectedBody.position).normalized * _dragDistanceFromAnchor;

            transform.position = isWithinDragDistance ? clampedTargetPos : targetPos;
            yield return null;
        }
    }
    private IEnumerator ShootRoutine()
    {
        if (!_anchorJoint)
        {
            Debug.LogError("No anchor connected to bird");
            yield break;
        }

        // every frame checks if mouse btn was let go
        while (!_wasShot)
        {
            if (Vector3.Distance(transform.position, _anchorJoint.connectedBody.position) < _releaseDistanceFromAnchor)
            {
                ShootBird();
                yield break;
            }
            yield return null;
        }
    }
    
    private void StartAiming()
    {
        // do sfx
        // do animation
        _rb.isKinematic = true;
        StartCoroutine(AimRoutine());
    }
    private void StopAiming()
    {
        _rb.isKinematic = false;
        StartCoroutine(ShootRoutine());
    }
    private void ShootBird()
    {
        PlayShotSound();
        _wasShot = true;
        _anchorJoint.connectedBody = null;
        _anchorJoint.breakForce = 0;
        _anchorJoint = null;

        EventManager.InvokeBirdShot();
    }
    private void Hit(Collision collision)
    {
        _deathTimer = _timeToDie;
        PlayImpactSound();

        if (_birdType == BirdType.Red) // play red animation
        {
            for (int i = 0; i < _animators.Length; i++)
            {
                _animators[i].SetTrigger("Impact");
            }
        }

        Vector3 collisionImpulse = collision.impulse;
        float impactForce = collisionImpulse.magnitude / Time.fixedDeltaTime;

        // set impact vfx direction to the opposite force
        ContactPoint contactPoint = collision.GetContact(0);
        Quaternion quaternion = Quaternion.LookRotation(contactPoint.normal); // calculate rotation based on reversed impact direction
        ParticleSystem feathersVFX = Instantiate(_feathersImpactVFX, contactPoint.point, quaternion);

        // set feathersVXF size according to the force of impact
        Vector3 newScale = feathersVFX.transform.localScale;
        newScale *= 1.0f + 1.0f / impactForce;
        feathersVFX.transform.localScale = newScale;

        ScoreManager scoreManager = ScoreManager.Instance;
        scoreManager.AddScore(20); // temp arbitrary score
        AnalyticsManager.Instance.UpdateScore(scoreManager.Score);

        _isHit = true;
    }

    private void PlaySpawnSound()
    {
        int audioIndex = UnityEngine.Random.Range(0, _spawnSounds.Length);
        _audioSource.PlayOneShot(_spawnSounds[audioIndex]); 
    }
    private void PlayShotSound()
    {
        int audioIndex = UnityEngine.Random.Range(0, _shotAudioClips.Length);
        _audioSource.PlayOneShot(_shotAudioClips[audioIndex]);
    }
    private void PlayImpactSound()
    {
        int audioIndex = UnityEngine.Random.Range(0, _impactAudioClips.Length);
        _audioSource.PlayOneShot(_impactAudioClips[audioIndex]);
    }
    #endregion

    #region Testings
    private void TestMousePos(Vector2 pointerPos)
    {
        Debugger.Log(pointerPos);
        Debugger.Log(Camera.main.ScreenPointToRay(pointerPos));
        Debugger.Log(Camera.main.ScreenPointToRay(Mouse.current.position.value));
    }

    [ContextMenu("Do Impact Animation")]
    private void TestAnimation()
    {
        _animators[0].SetTrigger("Impact");
        _animators[1].SetTrigger("Impact");
    }

    private void OnDrawGizmos()
    {
        if (_pointer != null)
        {
            float distance = Mathf.Abs(_mainCam.transform.position.z) + 1;
            Vector3 origin = _mainCam.ScreenToWorldPoint(_pointerPos);
            Vector3 direction = (origin - _mainCam.transform.position).normalized;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + direction * distance);
        }
    }
    #endregion
}
