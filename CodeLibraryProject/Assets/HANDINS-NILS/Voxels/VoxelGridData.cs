using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VoxelGridData : ScriptableObject
{
    public SerializableDictionary<int ,VoxelContainer> AllVoxels = new SerializableDictionary<int, VoxelContainer>();
    public SerializableDictionary<int, VoxelContainer> ColliderVoxels = new SerializableDictionary<int, VoxelContainer>();
    [HideInInspector] public SerializableDictionary<int, VoxelContainer> TraversableVoxels = new SerializableDictionary<int, VoxelContainer>();
    [HideInInspector] public List<SerializableDictionary<int, OctreeContainer>> OctreeDivisions = new List<SerializableDictionary<int, OctreeContainer>>();

    public float VoxelSize;
    public float[] MapDimensions;
}