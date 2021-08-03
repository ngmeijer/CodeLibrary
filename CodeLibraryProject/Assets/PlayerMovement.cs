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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraController = GetComponentInChildren<CameraController>();
    }

    private void Update()
    {
        trackInput();
        trackHorizontalRotation();
    }

    private void trackInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;

        transform.Translate(movement * (moveSpeed * Time.deltaTime));
    }

    private void trackHorizontalRotation()
    {
        float horiRot = Input.GetAxisRaw("Mouse X");
        transform.Rotate(transform.up, horiRot * cameraController.MouseSensitivity * Time.deltaTime);
    }
}