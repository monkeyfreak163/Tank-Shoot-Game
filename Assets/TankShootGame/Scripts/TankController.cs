using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    private FixedJoystick joystick;
    private Vector2 move;
    private Rigidbody2D rb;

    void Start()
    {
        // Get references to joystick and Rigidbody2D
        joystick = GameManager.Instance.tankControllJoystcick;
        rb = GameManager.Instance.Tanker.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // Read input from joystick
        move.x = joystick.Horizontal;
    }
    
    private void FixedUpdate()
    {
        // Move the tank based on the move direction and speed
        rb.MovePosition(rb.position + GameManager.Instance.TankerMoveSpeed * Time.fixedDeltaTime * move);
    }
}
