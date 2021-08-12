using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //
    private static InputManager _instance;
    public static InputManager Instance => _instance;
    //

    private List<BlockType> blocksInInventory;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public float HorizontalRotation { get; private set; }
    public float VerticalRotation { get; private set; }
    public float MouseWheel { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");
        
        HorizontalRotation = Input.GetAxisRaw("Mouse X");
        VerticalRotation = Input.GetAxisRaw("Mouse Y");
        
        MouseWheel = Input.GetAxisRaw("Mouse ScrollWheel");
    }
}
