using UnityEngine;

public class ShootCannaon : MonoBehaviour
{
    public FixedJoystick movementJoystick; // Reference to the movement joystick
    public FixedJoystick controlJoystick; // Reference to the control joystick
    public GameObject projectilePrefab; // Reference to the projectile prefab
    public LineRenderer trajectoryRenderer; // Reference to the LineRenderer for drawing trajectory
    public Transform shootingPoint; // Reference to the point from where the projectile is shot
    public float maxPower = 10f; // Maximum power for shooting
    public float minPower = 0f; // Minimum power for shooting
    public float defaultAngle = 45f; // Default shooting angle
    public float maxAngle = 90f; // Maximum angle for shooting
    public float minAngle = 0f; // Minimum angle for shooting
    public float trajectoryPointSpacing = 0.1f; // Spacing between trajectory points
    public int maxTrajectoryPoints = 100; // Maximum number of trajectory points

    private float power;
    private float angle;

    void Update()
    {
        // Get input from the movement joystick for tank movement
        //float horizontalInput = movementJoystick.Horizontal;
        //float verticalInput = movementJoystick.Vertical;

        //// Calculate movement based on input (example: move the tank forward)
        //Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
        //transform.Translate(movement * Time.deltaTime);

        // Get input from the control joystick for power and angle
        float controlHorizontalInput = controlJoystick.Horizontal;
        float controlVerticalInput = controlJoystick.Vertical;

        // Calculate power and angle based on control joystick input
        power = Mathf.Clamp(controlJoystick.Vertical, minPower, maxPower);
        angle = Mathf.Clamp(controlJoystick.Horizontal * maxAngle, minAngle, maxAngle);

        // Draw trajectory
        DrawTrajectory();

        // Check if fire button is pressed
        //if (controlJoystick.Pressed)
        //{
        //    // Shoot projectile
        //    Shoot();
        //}
    }

    void DrawTrajectory()
    {
        Vector3 velocity = Quaternion.Euler(0, angle, 0) * transform.forward * power;
        Vector3 currentPosition = shootingPoint.position;

        trajectoryRenderer.positionCount = 1;
        trajectoryRenderer.SetPosition(0, currentPosition);

        for (int i = 1; i < maxTrajectoryPoints; i++)
        {
            float time = i * trajectoryPointSpacing;
            Vector3 gravity = Physics.gravity;
            Vector3 newPosition = currentPosition + velocity * time + 0.5f * gravity * time * time;

            if (Physics.Raycast(currentPosition, newPosition - currentPosition, out RaycastHit hit, (newPosition - currentPosition).magnitude))
            {
                trajectoryRenderer.positionCount = i + 1;
                trajectoryRenderer.SetPosition(i, hit.point);
                break;
            }

            trajectoryRenderer.positionCount = i + 1;
            trajectoryRenderer.SetPosition(i, newPosition);
        }
    }

    void Shoot()
    {
        // Instantiate projectile
        GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);

        // Set projectile's velocity based on power and angle
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        Vector3 velocity = Quaternion.Euler(0, angle, 0) * transform.forward * power;
        projectileRb.velocity = velocity;
    }
}
