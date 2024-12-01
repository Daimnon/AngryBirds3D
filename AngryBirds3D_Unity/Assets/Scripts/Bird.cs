using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird : MonoBehaviour
{
    private InputSystem_Actions _controls;
    private InputAction _interactAction;

    private Camera _mainCam;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private SpringJoint _anchorJoint;
    [SerializeField][Range(0.1f, 100.0f)] private float _speed = 50.0f;
    [SerializeField][Range(0.1f, 100.0f)] private float _releaseDistanceFromAnchor = 0.4f;
    [SerializeField] private float _dragDistanceFromAnchor = 2.0f;
    [SerializeField] private LayerMask _interactableLayer;

    private Mouse _pointer;
    private Vector3 _pointerPos = Vector2.zero;

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
            //Debug.Log(hit.transform.name);
            StartAiming();
        }
    }
    private void OnPointerRelease(InputAction.CallbackContext context)
    {

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
        StopAiming();
    }
    private void StartAiming()
    {
        _rb.isKinematic = true;
        StartCoroutine(AimRoutine());
    }
    private void StopAiming()
    {
        _rb.isKinematic = false;
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
