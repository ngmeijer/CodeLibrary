using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private CameraController cameraController;

    [SerializeField] [Range(1, 10)] private float moveSpeed = 5f;
    [SerializeField] [Range(1, 5)] private float acceleration = 2f;
    [SerializeField] [Range(2, 20)] private float maxSpeed = 5f;
    [SerializeField] [Range(1, 10)] private float jumpForce = 2f;
    [SerializeField] private bool godMode = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraController = GetComponentInChildren<CameraController>();
        
        if (godMode) rb.useGravity = false;
    }

    private void Update()
    {
        trackInput();
        trackHorizontalRotation();
        trackJumpInput();
    }

    private void trackInput()
    {
        float horizontal = InputManager.Instance.Horizontal;
        float vertical = InputManager.Instance.Vertical;

        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;

        transform.Translate(movement * (moveSpeed * Time.deltaTime));
    }

    private void trackHorizontalRotation()
    {
        float horiRot = InputManager.Instance.HorizontalRotation;
        transform.Rotate(transform.up, horiRot * cameraController.MouseSensitivity * Time.deltaTime);
    }

    private void trackJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
}