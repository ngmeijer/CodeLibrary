using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI modeText;

    [SerializeField] private List<string> inventoryBlocks;
    [SerializeField] private List<Image> inventoryBlockUI = new List<Image>();
    [SerializeField] private Transform inventoryItemParent;
    private int index;

    private void Start()
    {
        int childCount = inventoryItemParent.childCount;

        for (int i = 0; i < childCount; i++)
        {
            GameObject itemParent = inventoryItemParent.GetChild(i).gameObject;
            inventoryBlockUI.Add(itemParent.transform.GetChild(1).GetComponent<Image>());
        }

        inventoryBlocks = GameManager.Instance.GetBlockNames();
        UpdateBlockInventoryUI();
    }

    private void Update()
    {
        CycleThroughInventoryUI();
    }

    public void ChangeEditMode(int pType)
    {
        switch (pType)
        {
            case 0:
                modeText.text = "Place/remove";
                break;
            case 1:
                modeText.text = "Shoot";
                break;
        }
    }

    public void UpdateBlockInventoryUI()
    {
        int imageIndex = 0;
        foreach (Image img in inventoryBlockUI)
        {
            string currentType = inventoryBlocks[imageIndex];
            imageIndex++;
            switch (currentType)
            {
                case "Grass":
                    img.color = Color.green;
                    break;
                case "Stone":
                    img.color = Color.grey;
                    break;
                case "LightWood":
                    img.color = Color.yellow;
                    break;
                case "DarkWood":
                    img.color = Color.red;
                    break;
                case "Dirt":
                    img.color = Color.black;
                    break;
                case "BlockTypeCount":
                    Debug.Log($"Invalid block ({currentType} in inventory.");
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void CycleThroughInventoryUI()
    {
        float mouseWheel = InputManager.Instance.MouseWheel;
        if (mouseWheel < 0) index--;
        if (mouseWheel > 0) index++;
    }
}