using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehavior : MonoBehaviour
{
    // Detect collision with other objects
    void OnCollisionEnter2D(Collision2D col)
    {
        // Check if collided with specific layers
        if (col.gameObject.layer == 6 || col.gameObject.layer == 7)
        {
            // Disable the bomb object
            gameObject.SetActive(false);

            // Reset all game elements using GameManager
            GameManager.Instance.ResetAll();
        }
    }
}
