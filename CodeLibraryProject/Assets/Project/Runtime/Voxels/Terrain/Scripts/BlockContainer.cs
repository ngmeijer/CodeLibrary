using System;
using UnityEngine;

[Serializable]
public class BlockContainer
{
    public Vector3 WorldPosition;
    public string BlockType;
    public VoxelContainer voxel;
}