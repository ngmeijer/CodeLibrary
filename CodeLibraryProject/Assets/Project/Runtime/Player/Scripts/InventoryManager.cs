using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<BlockType> blocksInInventory = new List<BlockType>();

    private void Start()
    {
        GameManager.Instance.ReceiveBlockInventory(blocksInInventory);
    }
}