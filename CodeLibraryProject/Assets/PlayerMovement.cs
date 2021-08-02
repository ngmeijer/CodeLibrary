using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float maxSpeed = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        trackInput();
    }

    private void trackInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 velocity = rb.velocity.normalized * moveSpeed;

        if (horizontal != 0)
        {
            if (velocity.x < -maxSpeed || velocity.x > maxSpeed) return;

            Vector3 movementVec = new Vector3(horizontal, 0, 0).normalized;

            if (horizontal > 0) velocity += movementVec * (moveSpeed * acceleration * Time.deltaTime);
            if (horizontal < 0) velocity += movementVec * (moveSpeed * acceleration * Time.deltaTime);
        }
        else velocity = new Vector3(0, velocity.y, velocity.z);

        if (vertical != 0)
        {
            if (rb.velocity.z < -maxSpeed || rb.velocity.z > maxSpeed) return;

            Vector3 movementVec = new Vector3(0, 0, vertical).normalized;

            if (vertical < 0) velocity += movementVec * (moveSpeed * acceleration * Time.deltaTime);
            if (vertical > 0) velocity += movementVec * (moveSpeed * acceleration * Time.deltaTime);
        }
        else velocity = new Vector3(velocity.x, velocity.y, 0);

        rb.velocity = velocity;
    }
}