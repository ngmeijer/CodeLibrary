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
        trackMovementInput();
        trackHorizontalRotation();
        trackJumpInput();
        if(godMode) trackDescendingInput();
    }

    private void trackMovementInput()
    {
        float horizontal = InputManager.Inst.Horizontal;
        float vertical = InputManager.Inst.Vertical;

        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;

        transform.Translate(movement * (moveSpeed * Time.deltaTime));
    }

    private void trackHorizontalRotation()
    {
        float horiRot = InputManager.Inst.HorizontalRotation;
        transform.Rotate(transform.up, horiRot * cameraController.MouseSensitivity * Time.deltaTime);
    }

    private void trackJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !godMode)
        { 
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKey(KeyCode.Space) && godMode)
        {
            transform.Translate(new Vector3(0, 1, 0) * (10 * Time.deltaTime), Space.World);
        }
    }

    private void trackDescendingInput()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.Translate(new Vector3(0, -1, 0) * (10 * Time.deltaTime), Space.World);
        }
    }
}