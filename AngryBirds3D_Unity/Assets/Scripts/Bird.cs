using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird : MonoBehaviour
{
    private InputSystem_Actions _controls;
    private InputAction _interactAction;

    [SerializeField] private Rigidbody _rb;
    [SerializeField][Range(0.1f, 100.0f)] private float _speed = 50.0f;
    [SerializeField][Range(0.1f, 100.0f)] private float _releaseDistanceFromAnchor = 2.0f;

    [SerializeField] private SpringJoint _anchorJoint;

    #region Monobehaviour Callbacks
    private void Awake()
    {
        _controls = new();
        InitializeInputs();
    }
    private void OnEnable()
    {
        EnableInputs();
    }
    private void OnDisable()
    {
        DisableInputs();
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
        StartAiming();
    }
    private void OnPointerRelease(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Behaviours
    private IEnumerator AimRoutine()
    {
        if (!_anchorJoint) yield break; Debug.LogError("No anchor connected to bird");

        // every frame checks if mouse btn was let go
        while (!_interactAction.WasReleasedThisFrame()) 
        {
            float distanceFronAnchor = Vector3.Distance(_anchorJoint.transform.position, transform.position);
            distanceFronAnchor = Mathf.Abs(distanceFronAnchor);

            if (distanceFronAnchor <= _releaseDistanceFromAnchor)
            {
                Debug.Log("Shot bird");
                yield break; 
            }
            yield return null;
        }
    }
    private void StartAiming()
    {
        _rb.isKinematic = true;
        StartCoroutine(AimRoutine());
    }
    #endregion
}
