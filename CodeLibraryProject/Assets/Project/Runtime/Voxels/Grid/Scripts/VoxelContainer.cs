using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VoxelContainer
{
    public int ID;
    public Vector3 WorldPosition;
    public Vector3Int GridPosition;
    public bool IsTraversable = true;

    public GameObject BlockInstance;
    
    public int Fcost;
    public int Gcost;
    public int Hcost;
    
    public List<int> NeighbourVoxelIDs;

    public VoxelContainer Parent;
}
