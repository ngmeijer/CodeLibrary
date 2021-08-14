using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    //
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    //

    private List<BlockType> blocksInInventory;
    private List<string> blockTypeNames = new List<string>();
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void ReceiveBlockInventory(List<BlockType> pBlocks)
    {
        blocksInInventory = pBlocks;
        foreach (BlockType type in blocksInInventory)
        {
            blockTypeNames.Add(type.ToString());
        }
    }

    public List<string> GetBlockNames() => blockTypeNames;
}