using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
[RequireComponent(typeof(VoxelMapVisualization), typeof(VoxelObstacleCalculator))]
public class VoxelGridCalculator : MonoBehaviour
{
    [Space(150)] [SerializeField] [Range(1, 1000)]
    private float sceneWidth = 50;

    [SerializeField] [Range(1, 100)] private float sceneHeight = 50;
    [SerializeField] [Range(1, 1000)] private float sceneDepth = 50;
    [SerializeField] [Range(1f, 50f)] private float voxelSize;
    [SerializeField] [Range(1, 5)] public int meshColliderAccuracy = 4;

    [ReadOnlyInspector] [SerializeField] private int totalExpectedVoxels = 0;

    public float VoxelSize { get; private set; }
    public VoxelGridData voxelGridSaveFile;

    private Vector3 pos;
    private VoxelObstacleCalculator collisionChecker;

    private void Awake()
    {
        collisionChecker = GetComponent<VoxelObstacleCalculator>();
    }

    private void Update()
    {
        totalExpectedVoxels = (int) ((sceneWidth / voxelSize) * (sceneHeight / voxelSize) * (sceneDepth / voxelSize));
    }

    public void RecalculateVoxelGrid()
    {
        if (voxelGridSaveFile == null)
        {
            Debug.Log("VoxelGridData save file reference is null.");
            return;
        }

        if (collisionChecker == null)
        {
            Debug.Log("VoxelObstacleCalculator reference is null.");
            return;
        }

        ClearVoxelData();
        divideLevelIntoVoxels();
        collisionChecker.StartCollisionCheck();
    }

    public void ClearVoxelData()
    {
        Console.Clear();
        voxelGridSaveFile.AllVoxels.Clear();
        voxelGridSaveFile.ColliderVoxels.Clear();
        voxelGridSaveFile.TraversableVoxels.Clear();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearDirty(voxelGridSaveFile);
#endif
    }

    private void divideLevelIntoVoxels()
    {
        VoxelSize = voxelSize;

        float voxelCountX = sceneWidth / voxelSize;
        float voxelCountY = sceneHeight / voxelSize;
        float voxelCountZ = sceneDepth / voxelSize;

        pos = transform.position;
        voxelGridSaveFile.VoxelSize = voxelSize;
        voxelGridSaveFile.MapDimensions = new float[3] {sceneWidth, sceneHeight, sceneDepth};

        int voxelID = 0;

        for (int x = 0; x < voxelCountX; x++)
            for (int y = 0; y < voxelCountY; y++)
                for (int z = 0; z < voxelCountZ; z++)
                {
                    VoxelContainer voxel = new VoxelContainer
                    {
                        Position = new Vector3(pos.x + (voxelSize * x), pos.y + (voxelSize * y),
                            pos.z + (voxelSize * z)),
                        ID = voxelID
                    };
                    voxelID++;

                    voxelGridSaveFile.AllVoxels.Add(voxel.ID, voxel);
                    voxelGridSaveFile.TraversableVoxels.Add(voxel.ID, voxel);
                }
        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(voxelGridSaveFile);
#endif
    }

    public void CalculateNeighboursAfterCollisionDetection()
    {
        foreach (KeyValuePair<int, VoxelContainer> voxel in voxelGridSaveFile.TraversableVoxels)
        {
            VoxelContainer currentVoxel = voxel.Value;
            List<int> neighbourVoxelIDs = calculateNeighbourVoxels(currentVoxel);
            currentVoxel.NeighbourVoxelIDs = neighbourVoxelIDs;
        }
    }

    private Vector3[] defineNeighbourVoxelPositions(Vector3 voxelPos)
    {
        Vector3[] positions =
        {
            //North
            new Vector3(voxelPos.x, voxelPos.y + voxelSize, voxelPos.z),

            //East
            new Vector3(voxelPos.x + voxelSize, voxelPos.y, voxelPos.z),

            //South
            new Vector3(voxelPos.x, voxelPos.y - voxelSize, voxelPos.z),

            //West
            new Vector3(voxelPos.x - voxelSize, voxelPos.y, voxelPos.z),

            //Center
            new Vector3(voxelPos.x, voxelPos.y, voxelPos.z + voxelSize),
            new Vector3(voxelPos.x, voxelPos.y, voxelPos.z - voxelSize),
        };

        return positions;
    }


    private List<int> calculateNeighbourVoxels(VoxelContainer pCurrentVoxel)
    {
        List<int> neighbourVoxels = new List<int>();

        float[] dimensions = GetMapDimensions();
        Vector3[] directions = defineNeighbourVoxelPositions(pCurrentVoxel.Position);

        for (int i = 0; i < directions.Length; i++)
        {
            VoxelContainer voxel = VoxelPositionHandler.GetVoxelFromWorldPos(voxelGridSaveFile.AllVoxels, directions[i],
                transform.position, dimensions, voxelSize);

            if ((voxel != null) && (!neighbourVoxels.Contains(voxel.ID)))
                neighbourVoxels.Add(voxel.ID);
        }

        return neighbourVoxels;
    }

    public float[] GetMapDimensions()
    {
        float[] dimensions =
        {
            sceneWidth,
            sceneHeight,
            sceneDepth
        };

        return dimensions;
    }

    public float GetVoxelSize() => voxelSize;
}