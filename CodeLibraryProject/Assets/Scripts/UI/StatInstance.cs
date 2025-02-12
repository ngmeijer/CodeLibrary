using TMPro;
using UnityEngine;

public class StatInstance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statName;
    [SerializeField] private TextMeshProUGUI _statValue;

    public void SetStatName(string name)
    {
        _statName.SetText(name);
    }

    public void SetStatValue(float value)
    {
        _statValue.SetText(value.ToString("F1"));
    }
}