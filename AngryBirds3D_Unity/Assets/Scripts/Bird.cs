using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird : MonoBehaviour
{
    private InputSystem_Actions _controls;
    private InputAction _lookAction, _interactAction;

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private SpringJoint _anchorJoint;
    [SerializeField][Range(0.1f, 100.0f)] private float _speed = 50.0f;
    [SerializeField][Range(0.1f, 100.0f)] private float _releaseDistanceFromAnchor = 2.0f;
    [SerializeField] private LayerMask _interactableLayer;

    private Mouse _pointer;
    private Vector2 _pointerPos = Vector2.zero;

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
        _pointer = Mouse.current;
    }
    private void Update()
    {
    }
    #endregion

    #region Inputs Handling
    private void InitializeInputs()
    {
        _lookAction = _controls.Player.Look;
        _interactAction = _controls.Player.Interact;
    }
    private void EnableInputs()
    {
        _lookAction.Enable();
        _interactAction.Enable();

        _controls.Player.Look.performed += OnLook;
        _controls.Player.Interact.started += OnPointerClick;
        _controls.Player.Interact.canceled += OnPointerRelease;
    }
    private void DisableInputs()
    {
        _controls.Player.Look.performed -= OnLook;
        _controls.Player.Interact.started -= OnPointerClick;
        _controls.Player.Interact.canceled -= OnPointerRelease;

        _interactAction.Disable();
        _lookAction.Disable();
    }
    #endregion

    #region Inputs Logic
    private void OnLook(InputAction.CallbackContext context)
    {
        _pointerPos = context.ReadValue<Vector2>();
        Debug.Log(_pointerPos);
    }
    private void OnPointerClick(InputAction.CallbackContext context)
    {
        Camera mainCam = Camera.main;

        float distance = Mathf.Abs(mainCam.transform.position.z) + 1;
        Vector3 newPointerVector = new(_pointerPos.x, _pointerPos.y, mainCam.nearClipPlane);
        Vector3 origin = mainCam.ScreenToWorldPoint(newPointerVector);
        Vector3 direction = (origin - mainCam.transform.position).normalized;

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
            float distanceFromAnchor = Vector3.Distance(transform.position, _anchorJoint.connectedBody.position);
            Vector3 newPointerVector = new(_pointerPos.x, _pointerPos.y, 0);
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(newPointerVector);
            transform.position = targetPos;
            yield return null;
        }
    }
    private void StartAiming()
    {
        _rb.isKinematic = true;
        StartCoroutine(AimRoutine());
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
            Camera mainCam = Camera.main;

            float distance = Mathf.Abs(mainCam.transform.position.z) + 1;
            Vector3 newPointerVector = new(_pointerPos.x, _pointerPos.y, mainCam.nearClipPlane);
            Vector3 origin = mainCam.ScreenToWorldPoint(newPointerVector);
            Vector3 direction = (origin - mainCam.transform.position).normalized;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + direction * distance);
        }
    }

}
