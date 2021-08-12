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

    [SerializeField] private List<Transform> possibleWaypoints;
    
    public Dictionary<int, VoxelContainer> GetAllTraversableVoxels() => 
        voxelGridSaveFile.TraversableVoxels;

    public Dictionary<Vector3, int> GetVoxelPositions() => voxelGridSaveFile.VoxelPositions;

    public List<Transform> getPossibleWaypoints() => possibleWaypoints;

    public Vector3 GetMapDimensions() => voxelGridSaveFile.MapDimensions;

    public float GetVoxelSize() => voxelGridSaveFile.VoxelSize;
}