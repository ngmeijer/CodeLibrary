using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<BlockType> blocksInInventory = new List<BlockType>();

    private void Awake()
    {
        GameManager.Inst.ReceiveBlockInventory(blocksInInventory);
    }
}