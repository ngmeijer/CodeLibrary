using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class StatsController : MonoBehaviour
{
    [SerializeField] private PlayerStats _statsData;

    public static UnityEvent<PlayerStats> OnReceivedStatsData = new();

    private void Start()
    {
        if (_statsData == null)
        {
            Debug.LogError("Stats SO is not assigned.");
        }
        OnReceivedStatsData?.Invoke(_statsData);
    }
}

[Serializable]
public class StatData
{
    public string Name;
    public float StatValue;
}