using System;
using UnityEngine;

public class GameplayUIState : UIState
{
    [SerializeField] private GameObject _gameplayUIContainer;
    
    public override void InitializeState()
    {
        
    }

    public override void EnterState()
    {
        _gameplayUIContainer.SetActive(true);
    }

    public override void ExitState()
    {
        _gameplayUIContainer.SetActive(false);
    }

    public override void UpdateState()
    {
        
    }
}