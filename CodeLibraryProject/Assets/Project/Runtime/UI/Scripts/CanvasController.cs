using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI modeText;

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

    public void UpdateBlockInventoryUI(List<BlockType> pBlocks)
    {
        
    }
}
