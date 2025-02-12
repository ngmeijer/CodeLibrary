using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum E_UIStates
{
    Stats,
    Inventory,
    Gameplay
}

public class UIManager : FSMManager<E_UIStates>
{
    protected override Dictionary<E_UIStates, FSMState> _states { get; set; } = new();
    private PlayerControls _controls;

    private void Awake()
    {
        UIState[] foundStates = FindObjectsByType<UIState>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (UIState state in foundStates)
        {
            _states.Add(state.SelectedState, state);
            state.ExitState();
        }
        
        SwitchToState(E_UIStates.Gameplay);
    }

    private void OnEnable()
    {
        _controls = new PlayerControls();
        
        _controls.UI.Enable();
        _controls.UI.EnableStatsUI.performed += HandleStatsUI;
        _controls.UI.EnableInventoryUI.performed += HandleInventoryUI;
    }
    
    private void OnDisable()
    {
        _controls.UI.EnableStatsUI.performed -= HandleStatsUI;
        _controls.UI.EnableInventoryUI.performed -= HandleInventoryUI;
        _controls.UI.Disable();
        
        _controls = null;
    }

    private void HandleStatsUI(InputAction.CallbackContext obj)
    {
        if(((UIState)_currentState).SelectedState == E_UIStates.Stats)
            SwitchToState(E_UIStates.Gameplay);
        else SwitchToState(E_UIStates.Stats);
    }
    
    private void HandleInventoryUI(InputAction.CallbackContext obj)
    {
        if(((UIState)_currentState).SelectedState == E_UIStates.Inventory)
            SwitchToState(E_UIStates.Gameplay);
        else SwitchToState(E_UIStates.Inventory);
    }
}