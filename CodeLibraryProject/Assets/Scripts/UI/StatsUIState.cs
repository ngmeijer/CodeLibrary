using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class StatsUIState : UIState
{
    [SerializeField] private GameObject _statsUIContainer;
    [SerializeField] private Transform _statsParent;
    [SerializeField] private GameObject _statsUIInstancePrefab;
    private SerializedDictionary<Stat, StatInstance> _statsUIInstances = new();

    public override void InitializeState()
    {
        StatsController.OnReceivedStatsData.AddListener(InitializeStatsUI);
        _statsUIContainer.SetActive(false);
    }

    private void InitializeStatsUI(PlayerStats receivedData)
    {
        receivedData.OnStatChanged += HandleStatInstance;
        foreach (KeyValuePair<Stat, StatData> stat in receivedData.GetAllStats())
        {
            HandleStatInstance(stat);
        }
    }

    private void HandleStatInstance(KeyValuePair<Stat, StatData> statKVP)
    {
        if (_statsUIInstances.ContainsKey(statKVP.Key))
        {
            _statsUIInstances[statKVP.Key].SetStatValue(statKVP.Value.StatValue);
            return;
        }

        GameObject instance = Instantiate(_statsUIInstancePrefab, _statsParent);
        StatInstance controller = instance.GetComponent<StatInstance>();
        controller.SetStatName(statKVP.Value.Name);
        controller.SetStatValue(statKVP.Value.StatValue);
        _statsUIInstances.Add(statKVP.Key, controller);
    }

    public override void EnterState()
    {
        _statsUIContainer.SetActive(true);
    }

    public override void ExitState()
    {
        _statsUIContainer.SetActive(false);
    }

    public override void UpdateState()
    {
        
    }
}