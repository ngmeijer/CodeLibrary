using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AI_Blackboard : MonoBehaviour
{
    [SerializeField] private VoxelGridData voxelGridSaveFile;
    [HideInInspector] public VoxelContainer targetVoxel;
    [HideInInspector] public VoxelContainer startVoxel;
    [HideInInspector] public List<VoxelContainer> path = new List<VoxelContainer>();
    
    public Dictionary<int, VoxelContainer> GetAllTraversableVoxels() => 
        voxelGridSaveFile.TraversableVoxels;
    

    public float[] GetMapDimensions() => voxelGridSaveFile.MapDimensions;

    public float GetVoxelSize() => voxelGridSaveFile.VoxelSize;
}