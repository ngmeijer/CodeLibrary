using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoxelContainer
{
    public int ID;
    public Vector3 Position;
    public bool IsTraversable = true;
    public Color ActiveColour;

    public List<int> NeighbourVoxelIDs;
    public VoxelContainer Parent;

    public List<int> neighbourIDs;
    public NeighbourData neighbourData = new NeighbourData();

    //total cost = G + H
    public int Fcost;

    //Distance between this voxel and start voxel
    public int Gcost;

    //Distance between this voxel and end voxel
    public int Hcost;
}

public class NeighbourData
{
    public Dictionary<int, VoxelContainer> neighbourVoxels = new Dictionary<int, VoxelContainer>();
}