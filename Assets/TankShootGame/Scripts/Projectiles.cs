using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectiles : MonoBehaviour
{
    // Variables for curve points
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform controlPoint1;
    [SerializeField] private Transform controlPoint2;
    [SerializeField] private Transform endPoint;
    private int numPoints = 50;

    // Variables for input handling
    private float horizontalInput;
    private float verticalInput;
    private FixedJoystick projectileJoystick;

    // Variables for projectile properties
    [SerializeField] LineRenderer lineRenderer;
    
    private float duration = 20f;
    [SerializeField] float maxDistance = 40;

    // Internal variables
    private Vector3[] positions;
    private Vector3 projectileEndPointPosition;
    private float inputMagnitudePower;
    [SerializeField] private float minMagnitudePowerSpeed = 3.2f, maxMagnitudePowerSpeed = 4.5f;
    private float minMagnitudeSpeed;

    //ObjectPool variables
    [SerializeField] GameObject projectilePrefab; // The prefab of the object you want to pool
    [SerializeField] private int poolSize = 10; // The initial size of the object pool
    private List<GameObject> pooledObjects = new List<GameObject>();
    private bool isFired;

    //Tank Animation variables
    [SerializeField]private Animator launchTankAnimation;

    void Start()
    {
        // Initialize line renderer and curve points
        lineRenderer.positionCount = numPoints;
        positions = new Vector3[numPoints];
        projectileEndPointPosition = endPoint.position;
        projectileJoystick = GameManager.Instance.projectileJoystick;

        // Populate the object pool with the specified number of objects
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
    private void OnEnable()
    {
        lineRenderer.gameObject.SetActive(false);
        GameManager.OnReset += ResetProjectail;
    }
    private void OnDisable()
    {
        GameManager.OnReset -= ResetProjectail;
    }

    void Update()
    {
        // Handle input
        HandleInput();

        // Update curve drawing
        DrawCurve();
    }
    public GameObject GetPooledObject()
    {
        // Iterate through the pool to find an inactive object to reuse
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true); // Activate the object
                return obj;
            }
        }

        // If no inactive object is found, create a new one and add it to the pool
        GameObject newObj = Instantiate(projectilePrefab);
        pooledObjects.Add(newObj);
        return newObj;
    }
    void DrawCurve()
    {
        // Draw Bezier curve
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            positions[i] = CalculateBezierPoint(t, startPoint.position, controlPoint1.position, controlPoint2.position, endPoint.position);
        }
        lineRenderer.SetPositions(positions);
    }
    void HandleInput()
    {
        if (projectileJoystick.enabled)
        {
            // Handle joystick input
            horizontalInput = projectileJoystick.Horizontal;
            verticalInput = projectileJoystick.Vertical;

            // Update end point position based on joystick input
            if (projectileJoystick.Direction != Vector2.zero)
            {
                
                lineRenderer.gameObject.SetActive(true);
                
                inputMagnitudePower = new Vector2(horizontalInput, verticalInput).magnitude;
                minMagnitudeSpeed=Mathf.Lerp(minMagnitudePowerSpeed,maxMagnitudePowerSpeed,inputMagnitudePower);

                Vector3 endPointPosition = new Vector3(horizontalInput, verticalInput, 0);
                endPoint.transform.position = endPointPosition * maxDistance;

                Vector3 midPoint1 = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, 0.35f);
                Vector3 midPoint2 = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, 0.7f);
                Vector3 endPos = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, 1f);

                Vector3 controlPoint1posison = new Vector3(midPoint1.x, 1, midPoint1.z);
                controlPoint1.transform.position = controlPoint1posison;

                Vector3 controlPoint2Position = new Vector3(midPoint2.x, 1f, midPoint2.z);
                controlPoint2.transform.position = controlPoint2Position;

                Vector3 endDefaultPosition = new Vector3(endPos.x, -2.93f, endPos.z);
                endPoint.transform.position = endDefaultPosition;
            }
        }
    }
    IEnumerator MoveBomb()
    {
        // Pool Instantiate bomb at start point and move it along the curve
        GameObject obj = GetPooledObject();
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;

            Vector3 position = CalculateBezierPoint(t, startPoint.position, controlPoint1.position, controlPoint2.position, endPoint.position);
            obj.transform.position = position;
            time += Time.deltaTime * minMagnitudeSpeed * 2.5f;

            yield return null;
        }
        obj.transform.position = endPoint.position;
        if(obj.transform.position == endPoint.position)
        {
            endPoint.position = projectileEndPointPosition;
        }
    }
    public void Shoot()
    {
        // Start shooting process
        if (!isFired)
        {
            launchTankAnimation.enabled = true;
            launchTankAnimation.SetTrigger("tack lauch effect");
            StartCoroutine(MoveBomb());
            isFired = true;
            lineRenderer.gameObject.SetActive(false);
            projectileJoystick.enabled = false;
        }
    }
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // Calculate point on Bezier curve
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;

        return point;
    }
    void ResetProjectail()
    {
        //Reset projectail
        launchTankAnimation.ResetTrigger("tack lauch effect");
        launchTankAnimation.enabled = false;
        isFired = false;
        projectileJoystick.enabled = true;
    }
}
