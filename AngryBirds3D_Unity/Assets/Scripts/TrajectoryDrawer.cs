using System;
using UnityEngine;

public class TrajectoryDrawer : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private SpringJoint _joint;
    [SerializeField] private LineRenderer _lr;
    [SerializeField] private int _linePointsCount = 15;
    [SerializeField] private float _timeBetweenPoints = 0.2f; // based on time

    [SerializeField] private float _maxTrhowForce; // based on time
    public float MaxTrhowForce { get => _maxTrhowForce; set => _maxTrhowForce = value; }

    private Vector3 _initialPos;
    public Vector3 InitialPos => _initialPos;

    private Vector3 _initialVel;
    public Vector3 InitialVel => _initialVel;

    private Vector3 _pointerPosOnDragStarted;
    public Vector3 PointerPosOnDragStarted => _pointerPosOnDragStarted;

    private float _initialForce;
    public float InitialForce => _initialForce;

    private void OnEnable()
    {
        EventManager.OnBirdShot += OnBirdShot;
    }
    private void OnDisable()
    {
        EventManager.OnBirdShot -= OnBirdShot;
    }

    private void Start()
    {
        _initialPos = _rb.position;
        _initialVel = _rb.linearVelocity;
        _pointerPosOnDragStarted = _rb.position;   
    }
    private void Update()
    {
        Draw();
    }

    private Vector3 CalculatePositionAtTime(Vector3 startPos, Vector3 startVel, float time)
    {
        float gravity = Physics.gravity.y;
        float x = startPos.x + startVel.x * time;
        float y = startPos.y + startVel.y * time + 0.5f * gravity * time * time;
        float z = startPos.z + startVel.z * time;
        return new Vector3(x, y, z);
    }

    public void Draw()
    {
        // Calculate how far we have pulled the object back (based on initial position and current position)
        float dragDistance = Vector3.Distance(_pointerPosOnDragStarted, _rb.position);

        // Estimate the launch velocity based on the drag distance and spring joint properties
        float springForce = Mathf.Clamp(dragDistance, 0f, _joint.maxDistance);
        float forceMultiplier = Mathf.Lerp(1f, _maxTrhowForce, springForce / _joint.maxDistance);

        // Set the launch velocity based on the force
        Vector3 launchVelocity = (_pointerPosOnDragStarted - _rb.position).normalized * springForce * forceMultiplier;

        // Calculate trajectory points
        Vector3[] trajectoryPoints = new Vector3[_linePointsCount];
        Vector3 startPosition = _rb.position;

        for (int i = 0; i < _linePointsCount; i++)
        {
            float time = i * _timeBetweenPoints;
            Vector3 positionAtTime = CalculatePositionAtTime(startPosition, launchVelocity, time);
            trajectoryPoints[i] = positionAtTime;
        }

        // Update LineRenderer
        _lr.positionCount = trajectoryPoints.Length;
        _lr.SetPositions(trajectoryPoints);
    }

    /*public void Draw() // works with line renderer after shooting
    {
        Vector3[] linePoints = new Vector3[_linePointsCount];
        Vector3 startPos = _rb.position;
        Vector3 startVel = _rb.linearVelocity;

        // draw path based on object's current velocity && position
        for (int i = 0; i < _linePointsCount; i++)
        {
            float time = i * _timeBetweenPoints;
            Vector3 positionAtTime = CalculatePositionAtTime(startPos, startVel, time);
            linePoints[i] = positionAtTime;
        }

        _lr.positionCount = linePoints.Length;
        _lr.SetPositions(linePoints);
    }*/

    private void OnBirdShot(Bird bird)
    {
        _initialForce = _rb.linearVelocity.magnitude;
    }
}
