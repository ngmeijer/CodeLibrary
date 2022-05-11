using System;
using UnityEngine;

[Serializable]
public class VoxelGridData : ScriptableObject
{
    [HideInInspector] public SerializableDictionary<int ,VoxelContainer> AllVoxels = new SerializableDictionary<int, VoxelContainer>();
    [HideInInspector] public SerializableDictionary<int, VoxelContainer> ColliderVoxels = new SerializableDictionary<int, VoxelContainer>();
    [HideInInspector] public SerializableDictionary<int, VoxelContainer> TraversableVoxels = new SerializableDictionary<int, VoxelContainer>();
    [HideInInspector] public SerializableDictionary<Vector3, int> VoxelPositions = new SerializableDictionary<Vector3, int>();
    
    public SceneData sceneData;
    public float VoxelCount;
    public float VoxelSize;
    public Vector3 MapDimensions;
}