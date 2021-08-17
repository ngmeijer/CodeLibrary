using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveDataProcessor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI dateTxt;

    public void ReceiveData(string pName, string pDate)
    {
        nameTxt.text = pName;
        dateTxt.text = pDate;
    }
}