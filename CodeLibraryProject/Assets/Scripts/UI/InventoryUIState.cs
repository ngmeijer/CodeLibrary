using System;
using UnityEngine;

public class InventoryUIState : UIState
{
    [SerializeField] private GameObject _inventoryUIContainer;

    public override void InitializeState()
    {
        
    }

    public override void EnterState()
    {
        _inventoryUIContainer.SetActive(true);
    }

    public override void ExitState()
    {
        _inventoryUIContainer.SetActive(false);
    }

    public override void UpdateState()
    {
        
    }
}