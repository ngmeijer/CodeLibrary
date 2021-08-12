using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float force;
    
    private void Start()
    {
        rb.AddForce(transform.forward * force);
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(this);
    }
}
