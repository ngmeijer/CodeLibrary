using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController : MonoBehaviour
{
    private Camera cam;
    private BlockUnitPlacer blockUnitPlacer;
    private RaycastHit testHit;

    private void Start()
    {
        cam = Camera.main;
        blockUnitPlacer = FindObjectOfType<BlockUnitPlacer>();
    }

    private void Update()
    {
        trackMouseInput();
    }

    private void trackMouseInput()
    {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.yellow);
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                testHit = hit;
                blockUnitPlacer.ReceiveSelectedVoxelPosition(hit.collider.transform.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (testHit.collider == null) return;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(testHit.transform.position, 1);
    }
}