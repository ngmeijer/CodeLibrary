using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeContainer
{
    public float MinVoxelSize;
    public Vector3 Position;
    public Vector3 OctreeNodeSize;
    public int NodeID;
    
    [HideInInspector] public OctreeContainer ParentContainer;
    [HideInInspector] public List<OctreeContainer> ChildrenContainers = new List<OctreeContainer>();
}
