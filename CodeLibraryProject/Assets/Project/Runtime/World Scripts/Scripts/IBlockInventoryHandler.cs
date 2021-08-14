using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlockInventoryHandler
{
    public void CycleThroughBlocks();
    public void LoadResourceFromAssets(string pType);
}
