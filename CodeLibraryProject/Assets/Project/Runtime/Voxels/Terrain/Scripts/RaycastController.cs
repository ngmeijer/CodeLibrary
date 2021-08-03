using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Place,
    Remove,
}

public class RaycastController : MonoBehaviour
{
    private Camera cam;
    private TerrainGenerator terrainGenerator;
    private RaycastHit testHit;

    private void Start()
    {
        cam = Camera.main;
        terrainGenerator = FindObjectOfType<TerrainGenerator>();
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
                terrainGenerator.HandleBlockAction(hit.collider.transform.position, ActionType.Place);
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                testHit = hit;
                terrainGenerator.HandleBlockAction(hit.collider.transform.position, ActionType.Remove, hit);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (testHit.collider == null) return;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(testHit.transform.position, 0.5f);
    }
}