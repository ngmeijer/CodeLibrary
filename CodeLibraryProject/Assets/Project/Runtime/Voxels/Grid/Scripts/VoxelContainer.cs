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

    //total cost = G + H
    public int Fcost;

    //Distance between this voxel and start voxel
    public int Gcost;

    //Distance between this voxel and end voxel
    public int Hcost;
    public List<int> NeighbourVoxelIDs;

    public VoxelContainer Parent;
}
