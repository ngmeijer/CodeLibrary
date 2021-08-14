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
    [SerializeField] private Transform inventoryItemParent;
    
    private List<Image> inventoryBlockBackground = new List<Image>();
    private List<Image> inventoryBlockTexture = new List<Image>();
    private int index = 1;

    private void Start()
    {
        int childCount = inventoryItemParent.childCount;

        for (int i = 0; i < childCount; i++)
        {
            GameObject itemParent = inventoryItemParent.GetChild(i).gameObject;
            inventoryBlockBackground.Add(itemParent.transform.GetChild(0).GetComponent<Image>());
            inventoryBlockTexture.Add(itemParent.transform.GetChild(1).GetComponent<Image>());
        }

        inventoryBlocks = GameManager.Instance.GetBlockNames();
        inventoryBlockBackground[0].color = Color.magenta;
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
        foreach (Image img in inventoryBlockTexture)
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
        if (mouseWheel == 0) return;
        
        if (mouseWheel < 0)
        {
            if (index <= 0)
                index = inventoryBlockTexture.Count - 1;
            else index--;
        }
        if (mouseWheel > 0)
        {
            if (index >= inventoryBlockTexture.Count - 1)
                index = 0;
            else index++;
        }

        inventoryBlockBackground[index].color = Color.magenta;
        Debug.Log(index);

        foreach (Image background in inventoryBlockBackground)
        {
            if (inventoryBlockBackground[index] == background) continue;

            background.color = new Color(0,0,0, 0.5f);
        }
    }
}