using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(10, 120)] public float MouseSensitivity = 90f;
    [SerializeField] private bool isInverted = true;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        trackVerticalRotation();
    }

    private void trackVerticalRotation()
    {
        float vertRot = InputManager.Instance.VerticalRotation;

        if (isInverted) vertRot *= -1;
        
        transform.Rotate(Vector3.right, vertRot * MouseSensitivity * Time.deltaTime);
    }
}