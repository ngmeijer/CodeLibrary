using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[Serializable]
public class Event_GetNameOfSaveGame : UnityEvent<string> { }

public class SaveDataProcessor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI dateTxt;
    public Toggle Toggle;

    private string saveName;
    private string dateCreated;

    public int Index;
    
    public void ReceiveData(string pName, string pDate)
    {
        saveName = pName;
        nameTxt.text = pName;

        dateCreated = pDate;
        dateTxt.text = pDate;
    }

    public void SendNameOnClick()
    {
        SavedGameLoader.Inst.SelectSaveGame(Index);
    }
}