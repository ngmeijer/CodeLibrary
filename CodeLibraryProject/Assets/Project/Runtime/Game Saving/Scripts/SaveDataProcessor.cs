using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveDataProcessor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI dateTxt;
    [SerializeField] private TextMeshProUGUI fileSizeTxt;

    public void ReceiveData(string pName, string pDate, string pSize)
    {
        nameTxt.text = pName;
        dateTxt.text = pDate;
        fileSizeTxt.text = pSize;
    }
}
