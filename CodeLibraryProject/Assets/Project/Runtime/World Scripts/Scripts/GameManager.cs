using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Event_BlockInventory : UnityEvent<BlockType>
{
    
}

[Serializable]
public enum BlockType
{
    Grass,
    Stone,
    LightWood,
    DarkWood,
    Dirt,
    BlockTypeCount
}

public class GameManager : MonoBehaviour
{
    //
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    //

    private List<BlockType> blocksInInventory;

    [SerializeField] private Event_BlockInventory blockInventoryNotification;
    
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
    }
}
