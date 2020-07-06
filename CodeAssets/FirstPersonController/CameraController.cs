using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform player = null;

    [SerializeField] private float rotateSpeed = 100f;

    private float xAxisClamp = 0f;

    [SerializeField] private KeyCode interactKey = KeyCode.F;

    #endregion

    private void Awake()
    {
        xAxisClamp = 0f;
    }

    private void LateUpdate()
    {
        lookUp();
    }

    //This method is located in the CameraController, because the player is a simple cylinder for now.
    //This means it's not possible to have it in the PlayerController without having to rotate the entire playerObject on the X-axis.
    private void lookUp()
    {
        float verticalLook = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        xAxisClamp += verticalLook;

        if (xAxisClamp > 90f)
        {
            xAxisClamp = 90f;
            verticalLook = 0f;
            cameraMaxRotation(270f);
        }
        else if (xAxisClamp < -90f)
        {
            xAxisClamp = -90f;
            verticalLook = 0f;
            cameraMaxRotation(90f);
        }

        //Remove "-" to invert look direction.
        //Also make this an option in the pause menu.
        transform.Rotate(-verticalLook, 0f, 0f);
    }

    private void cameraMaxRotation(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}
