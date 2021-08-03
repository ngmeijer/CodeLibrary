using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VoxelGridData : ScriptableObject
{
    [HideInInspector] public SerializableDictionary<int ,VoxelContainer> AllVoxels = new SerializableDictionary<int, VoxelContainer>();
    [HideInInspector] public SerializableDictionary<int, VoxelContainer> ColliderVoxels = new SerializableDictionary<int, VoxelContainer>();
    [HideInInspector] public SerializableDictionary<int, VoxelContainer> TraversableVoxels = new SerializableDictionary<int, VoxelContainer>();
    [HideInInspector] public SerializableDictionary<Vector3Int, int> VoxelPositions = new SerializableDictionary<Vector3Int, int>();

    public float VoxelCount;
    public float VoxelSize;
    public float[] MapDimensions;
}