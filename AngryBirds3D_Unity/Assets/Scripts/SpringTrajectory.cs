using UnityEngine;

public class SpringTrajectory : MonoBehaviour
{
    public LineRenderer lineRenderer;  // LineRenderer to visualize the trajectory
    public Rigidbody rb;              // Rigidbody of the object
    public SpringJoint springJoint;   // Reference to SpringJoint
    public Transform anchor;          // The anchor where the spring joint is attached
    public int resolution = 30;       // Number of points to calculate the trajectory
    public float timeStep = 0.1f;     // Time step for trajectory calculation
    public float forceMultiplier = 0.05f;  // Scaling factor to reduce the force

    private bool springBroken = false;  // Flag to check if the spring joint has broken

    private float _releaseDistanceFromAnchor = 0.75f;  // Threshold for spring release distance

    void Start()
    {
        EventManager.OnBirdShot += OnBirdShot;
        anchor = BirdManager.Instance.AnchorRb.transform;
    }

    private void OnBirdShot(Bird bird)
    {
        springBroken = true;
        lineRenderer.positionCount = 0;  // Stop drawing the trajectory after release
    }

    void Update()
    {
        if (!springBroken)
        {
            DrawTrajectory();
        }
    }

    void DrawTrajectory()
    {
        Vector3[] trajectoryPoints = new Vector3[resolution];

        // Calculate trajectory points before the spring breaks
        for (int i = 0; i < resolution; i++)
        {
            float time = i * timeStep;

            // Calculate the position at the current time based on the spring force
            Vector3 positionAtTime = CalculatePositionAtTime(time);
            trajectoryPoints[i] = positionAtTime;
        }

        // Update the LineRenderer with the calculated trajectory points
        lineRenderer.positionCount = trajectoryPoints.Length;
        lineRenderer.SetPositions(trajectoryPoints);
    }

    // Function to calculate position at a given time based on projectile motion equations
    Vector3 CalculatePositionAtTime(float time)
    {
        float g = Physics.gravity.y;  // Get gravity's Y value
        Vector3 startPos = rb.position;  // Position when the spring joint is attached

        // Calculate the spring force based on the current distance between the anchor and the object
        float springForce = springJoint.spring * (Vector3.Distance(anchor.position, rb.position));

        // Calculate the velocity imparted by the spring (scaled down by a multiplier)
        Vector3 velocity = (anchor.position - rb.position).normalized * springForce * forceMultiplier;

        // Calculate the position at time 't' based on projectile motion equations
        float x = startPos.x + velocity.x * time;
        float y = startPos.y + velocity.y * time + 0.5f * g * time * time;
        float z = startPos.z + velocity.z * time;

        return new Vector3(x, y, z);
    }
}