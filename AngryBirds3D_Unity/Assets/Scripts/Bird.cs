using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird : MonoBehaviour
{
    private InputSystem_Actions _controls;
    private InputAction _interactAction;

    private Camera _mainCam;

    [SerializeField] private BirdType _birdType;
    public BirdType @BirdType => _birdType;

    
    [SerializeField] private GameObject _anchorGO; // anchor is recieved when spawned form BirdManager
    public GameObject AnchorGO { get => _anchorGO; set => _anchorGO = value; }

    [SerializeField] private SpringJoint _anchorJoint; // joint connectedBody is recieved when spawned form BirdManager
    public SpringJoint AnchorJoint => _anchorJoint;

    [SerializeField] private Rigidbody _rb;
    [SerializeField][Range(0.1f, 100.0f)] private float _speed = 50.0f;
    [SerializeField][Range(0.1f, 100.0f)] private float _releaseDistanceFromAnchor = 0.75f;
    [SerializeField] private float _dragDistanceFromAnchor = 2.0f;
    [SerializeField] private LayerMask _interactableLayer;

    private Mouse _pointer;
    private Vector3 _pointerPos = Vector2.zero;
    private bool _wasShot = false; // if has been shot already

    #region Monobehaviour Callbacks

    private void OnEnable()
    {
        EnableInputs();
    }
    private void OnDisable()
    {
        DisableInputs();
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
        StopAiming();
    }
    #endregion

    #region Behaviours
    private IEnumerator AimRoutine()
    {
        if (!_anchorJoint)
        {
            Debug.LogError("No anchor connected to bird");
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
        _wasShot = true;
        _anchorJoint.connectedBody = null;
        Destroy(_anchorJoint);
        _anchorJoint = null;

        EventManager.InvokeBirdShot();
    }
    private void Hit()
    {
        // do sfx
        // do vfx
        // remove script / disable script
    }
    #endregion

    #region Testings
    private void TestMousePos(Vector2 pointerPos)
    {
        Debug.Log(pointerPos);
        Debug.Log(Camera.main.ScreenPointToRay(pointerPos));
        Debug.Log(Camera.main.ScreenPointToRay(Mouse.current.position.value));
    }
    #endregion

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

}
