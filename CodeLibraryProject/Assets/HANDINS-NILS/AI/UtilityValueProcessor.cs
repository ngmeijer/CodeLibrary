using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityValueProcessor : MonoBehaviour, IUtilityValue
{
    private float utility;
    
    public void OnUtilityChanged(float pUtilityValue)
    {
        utility = pUtilityValue;
    }
}

public interface IUtilityValue
{
    void OnUtilityChanged(float pUtilityValue);
}
