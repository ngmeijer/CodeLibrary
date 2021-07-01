using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VoxelGridData : ScriptableObject
{
    public Dictionary<int ,VoxelContainer> AllVoxels = new Dictionary<int, VoxelContainer>();
    public SerializableDictionary<int, VoxelContainer> ColliderVoxels = new SerializableDictionary<int, VoxelContainer>();
    [HideInInspector] public SerializableDictionary<int, VoxelContainer> TraversableVoxels = new SerializableDictionary<int, VoxelContainer>();
    
    public float VoxelSize;
    public float[] MapDimensions;
}