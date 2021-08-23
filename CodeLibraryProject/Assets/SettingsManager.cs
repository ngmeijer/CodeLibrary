using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    //
    private static SettingsManager _inst;
    public static SettingsManager Inst => _inst;
    //
    
    public bool AutoSaveOn { get; private set; }
    
    private void Awake()
    {
        if (_inst != null && _inst != this)
        {
            Destroy(this.gameObject);
        } else {
            _inst = this;
        }
    }

    public void HandleAutoSaveState(bool pState)
    {
        Debug.Log($"auto save is {pState}");
        AutoSaveOn = pState;
    }
}