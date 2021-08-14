using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour, IBlockInventoryHandler
{
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private Transform inventoryItemParent;

    private List<string> inventoryBlocks;
    private List<Image> inventoryBlockBackground = new List<Image>();
    private List<Image> inventoryBlockTexture = new List<Image>();
    private int index = 0;
    public SerializableDictionary<string, Sprite> blockUISprites;

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
        
        loadUIBlockTextures();
        UpdateBlockInventoryUI();
    }

    private void Update()
    {
        CycleThroughBlocks();
    }

    private void loadUIBlockTextures()
    {
        blockUISprites.Clear();

        LoadResourceFromAssets("Grass");
        LoadResourceFromAssets("Stone");
        LoadResourceFromAssets("DarkWood");
        LoadResourceFromAssets("Dirt");
        LoadResourceFromAssets("LightWood");
    }

    public void LoadResourceFromAssets(string pType)
    {
        const string path = "UI/Texture_";
        Sprite itemUISprite = Resources.Load <Sprite>(path + pType);
        blockUISprites.Add(pType, itemUISprite);
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
            Sprite sprite;
            
            switch (currentType)
            {
                case "Grass":
                    blockUISprites.TryGetValue("Grass", out sprite);
                    img.sprite = sprite;
                    break;
                case "Stone":
                    blockUISprites.TryGetValue("Stone", out sprite);
                    img.sprite = sprite;
                    break;
                case "LightWood":
                    blockUISprites.TryGetValue("LightWood", out sprite);
                    img.sprite = sprite;
                    break;
                case "DarkWood":
                    blockUISprites.TryGetValue("DarkWood", out sprite);
                    img.sprite = sprite;
                    break;
                case "Dirt":
                    blockUISprites.TryGetValue("Dirt", out sprite);
                    img.sprite = sprite;
                    break;
                case "BlockTypeCount":
                    Debug.Log($"Invalid block ({currentType} in inventory.");
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void CycleThroughBlocks()
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

        foreach (Image background in inventoryBlockBackground)
        {
            if (inventoryBlockBackground[index] == background) continue;

            background.color = new Color(0,0,0, 0.5f);
        }
    }
}