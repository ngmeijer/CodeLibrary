using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Place,
    Remove,
}

public enum InteractType
{
    PlaceRemove,
    Shoot,
}

public class RaycastController : MonoBehaviour
{
    private Camera cam;
    private TerrainGenerator terrainGenerator;
    private RaycastHit testHit;
    private InteractType interactType;
    private CanvasController canvasController;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    
    private void Start()
    {
        cam = Camera.main;
        terrainGenerator = FindObjectOfType<TerrainGenerator>();
        canvasController = FindObjectOfType<CanvasController>();
    }

    private void Update()
    {
        switchEditType();
        trackEditMode();
    }

    private void trackEditMode()
    {
        if (interactType == InteractType.PlaceRemove) trackPlaceRemoving();
        if (interactType == InteractType.Shoot) trackShooting();
    }

    private void trackShooting()
    {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.yellow);
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bulletInstance =
                Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
        }
    }

    private void trackPlaceRemoving()
    {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.yellow);
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                testHit = hit;
                terrainGenerator.HandleBlockAction(ActionType.Place, hit);
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                testHit = hit;
                terrainGenerator.HandleBlockAction(ActionType.Remove, hit);
            }
        }
    }

    private void switchEditType()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch (interactType)
            {
                case InteractType.PlaceRemove:
                    interactType = InteractType.Shoot;
                    break;
                case InteractType.Shoot:
                    interactType = InteractType.PlaceRemove;
                    break;
            }
        }
        
        canvasController.ChangeEditMode((int)interactType);
    }
    
    private void OnDrawGizmos()
    {
        if (testHit.collider == null) return;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(testHit.transform.position, 0.5f);
    }
}