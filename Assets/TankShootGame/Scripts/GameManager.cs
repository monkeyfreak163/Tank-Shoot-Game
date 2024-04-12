using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Joysticks
    public FixedJoystick projectileJoystick;
    public FixedJoystick tankControllJoystcick;

    // Tank settings
    public GameObject Tanker;
    public float TankerMoveSpeed=5;

    // Reset event
    public delegate void ResetDelegate();
    public static event ResetDelegate OnReset;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
    }

    // Method to reset all game elements
    public void ResetAll()
    {
        // Invoke the reset event
        OnReset?.Invoke();
    }
}
