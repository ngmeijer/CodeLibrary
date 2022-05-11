using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GameManager : MonoBehaviour
{
    //
    private static GameManager _inst;
    public static GameManager Inst => _inst;
    //

    private List<BlockType> blocksInInventory;
    private List<string> blockTypeNames = new List<string>();
    
    private void Awake()
    {
        if (_inst != null && _inst != this)
        {
            Destroy(this.gameObject);
        } else {
            _inst = this;
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