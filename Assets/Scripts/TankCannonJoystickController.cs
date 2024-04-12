using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class TankCannonJoystickController : MonoBehaviour
{
    public FixedJoystick controlJoystick; // Reference to the control joystick
    public Button fireButton; // Reference to the fire button
    public GameObject projectilePrefab; // Reference to the projectile prefab
    public LineRenderer trajectoryRenderer; // Reference to the LineRenderer for drawing the trajectory
    public Transform cannonLauncher;

    private Vector3 shootDirection;
    private Vector2 releaseVector;
    public float projectileSpeed = 10f;


    void Update()
    {
        if (controlJoystick.Direction != Vector2.zero)
        {
            // Get input from the control joystick for power and angle
            float power = controlJoystick.Vertical;
            float angle = Mathf.Atan2(controlJoystick.Direction.y, controlJoystick.Direction.x) * Mathf.Rad2Deg;
            cannonLauncher.rotation = Quaternion.Euler(0f, 0f, angle);

            // Draw trajectory
            DrawTrajectory(power, angle);

            // Save angle and power data
            releaseVector = controlJoystick.Direction;
        }
        // Draw trajectory based on power and angle
        //DrawTrajectory();
    }

    public void Fire()
    {
        Shoot(releaseVector);
    }

    void DrawTrajectory(float power, float angle)
    {
        // Calculate initial velocity vector
        Debug.Log(angle + "Angle");
        //angle = 45;
        Vector3 velocity = Quaternion.AngleAxis(angle, Vector2.right) * Vector2.one * power * projectileSpeed;

        // Simulate projectile motion for a short duration and draw trajectory
        Vector3 currentPosition = transform.position;
        trajectoryRenderer.positionCount = 2;
        trajectoryRenderer.SetPosition(0, currentPosition);
        for (int i = 1; i <= 50; i++)
        {
            float time = i * 0.003f; // Change this value to adjust the duration of the trajectory
            Vector3 newPosition = currentPosition + velocity * time + 0.5f * Physics.gravity * time * time;
            trajectoryRenderer.positionCount = i + 1;
            trajectoryRenderer.SetPosition(i, newPosition);
            currentPosition = newPosition;
        }
    }
    void Shoot(Vector2 releaseVector)
    {
        // Calculate angle and power based on saved releaseVector
        float power = releaseVector.magnitude *3;
        float angle = Mathf.Atan2(releaseVector.y, releaseVector.x) * Mathf.Rad2Deg;

        // Instantiate projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Set projectile's velocity based on power and angle
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.velocity = Quaternion.Euler(0, angle, 0) * transform.right * projectileSpeed * power;
    }


}
