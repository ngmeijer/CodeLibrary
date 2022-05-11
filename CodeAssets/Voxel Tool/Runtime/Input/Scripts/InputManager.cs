using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //
    private static InputManager _inst;
    public static InputManager Inst => _inst;
    //

    private List<BlockType> blocksInInventory;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public float HorizontalRotation { get; private set; }
    public float VerticalRotation { get; private set; }
    public float MouseWheel { get; private set; }
    public KeyCode PauseGameKey = KeyCode.Escape;

    public bool CanInteract { get; set; } = true;

    private void Awake()
    {
        if (_inst != null && _inst != this)
        {
            Destroy(this.gameObject);
        } else {
            _inst = this;
        }
    }

    private void Update()
    {
        if (!CanInteract) return;
        
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");
        
        HorizontalRotation = Input.GetAxisRaw("Mouse X");
        VerticalRotation = Input.GetAxisRaw("Mouse Y");
        
        MouseWheel = Input.GetAxisRaw("Mouse ScrollWheel");
    }
}
