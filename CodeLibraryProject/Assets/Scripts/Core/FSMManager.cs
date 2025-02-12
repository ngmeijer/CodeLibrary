using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMManager<TStateKey> : MonoBehaviour
{
    protected virtual Dictionary<TStateKey, FSMState> _states { get; set; }
    protected FSMState _currentState;

    private void Start()
    {
        foreach (KeyValuePair<TStateKey, FSMState> state in _states)
        {
            state.Value.InitializeState();
        }
    }

    protected void SwitchToState(TStateKey state)
    {
        if (_currentState == null)
        {
            _currentState = _states[state];
            _currentState.EnterState();
            return;
        }
        
        if (!_states.ContainsKey(state))
        {
            Debug.LogError($"No state called '{state} exists.'");
            return;
        }

        _currentState.ExitState();

        _currentState = _states[state];
        _currentState.EnterState();
    }
}