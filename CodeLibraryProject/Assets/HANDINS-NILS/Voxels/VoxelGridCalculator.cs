using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
[RequireComponent(typeof(VoxelMapVisualization), typeof(VoxelObstacleCalculator))]
public class VoxelGridCalculator : MonoBehaviour
{
    [Space(75)] [SerializeField] [Range(1, 1000)]
    private float sceneWidth = 50;

    [SerializeField] [Range(1, 500)] private float sceneHeight = 50;
    [SerializeField] [Range(1, 1000)] private float sceneDepth = 50;
    [ReadOnlyInspector] [SerializeField] private Vector3 sceneDimensionsVector;
    private float[] mapDimensionsFloat;

    [Space(25)] [SerializeField] [Range(1f, 50f)]
    private float voxelSize;
    
    public VoxelGridData voxelGridSaveFile;
    public string[] ColliderTagsToCompare;

    //Public variables

    //Private variables
    private Vector3 pos;
    private VoxelObstacleCalculator collisionChecker;
    private int voxelCountX;
    private int voxelCountY;
    private int voxelCountZ;

    [Header("Grid properties")]
    [ReadOnlyInspector] [SerializeField] private int totalExpectedVoxels = 0;
    [ReadOnlyInspector] [SerializeField] private int totalCurrentVoxels = 0;
    [ReadOnlyInspector] [SerializeField] private float calculationTimeMilliseconds;
    [ReadOnlyInspector] [SerializeField] private float calculationTimeSeconds;

    private void Awake()
    {
        collisionChecker = GetComponent<VoxelObstacleCalculator>();
    }

    private void Update()
    {
        voxelCountX = (int) Math.Ceiling((sceneWidth / voxelSize));
        voxelCountY = (int) Math.Ceiling((sceneHeight / voxelSize));
        voxelCountZ = (int) Math.Ceiling((sceneDepth / voxelSize));

        totalExpectedVoxels = voxelCountX * voxelCountY * voxelCountZ;
        sceneDimensionsVector = new Vector3(sceneWidth, sceneHeight, sceneDepth);
    }

    public void RecalculateVoxelGrid()
    {
        float startTime = Time.realtimeSinceStartup;

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
        collisionChecker.StartCollisionCheck(voxelSize);
        
        calculationTimeMilliseconds = (Time.realtimeSinceStartup - startTime) * 1000f;
        calculationTimeSeconds = (Time.realtimeSinceStartup - startTime);
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
        pos = transform.position;
        voxelGridSaveFile.VoxelSize = voxelSize;
        voxelGridSaveFile.MapDimensions = new float[3] {sceneWidth, sceneHeight, sceneDepth};

        int voxelID = 0;

        ProgressBar.MaxVoxelIndex = totalExpectedVoxels - 1;
        
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
                    ProgressBar.ShowVoxelCreateProgress(voxelID);

                    voxelGridSaveFile.AllVoxels.Add(voxel.ID, voxel);
                    voxelGridSaveFile.TraversableVoxels.Add(voxel.ID, voxel);

                    voxelID++;
                }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(voxelGridSaveFile);

        totalCurrentVoxels = voxelGridSaveFile.AllVoxels.Count;
        
        ProgressBar.HasFinishedProcess = true;
        ProgressBar.ShowVoxelCreateProgress(voxelID);
#endif
    }

    public void CalculateNeighboursAfterCollisionDetection()
    {
        int currentVoxelIndex = 0;
        ProgressBar.MaxVoxelIndex = voxelGridSaveFile.TraversableVoxels.Count - 1;
        mapDimensionsFloat = GetMapDimensions();
        foreach (KeyValuePair<int, VoxelContainer> voxel in voxelGridSaveFile.TraversableVoxels)
        {
            currentVoxelIndex++;
            VoxelContainer currentVoxel = voxel.Value;
            ProgressBar.ShowVoxelNeighbourProgress(currentVoxelIndex);
            List<int> neighbourVoxelIDs = calculateNeighbourVoxels(currentVoxel);
            currentVoxel.NeighbourVoxelIDs = neighbourVoxelIDs;
        }
        
        ProgressBar.HasFinishedProcess = true;
        ProgressBar.ShowVoxelNeighbourProgress(ProgressBar.MaxVoxelIndex);
    }

    private Vector3[] defineNeighbourVoxelPositions(Vector3 pVoxelPos)
    {
        Vector3[] positions =
        {
            //North
            new Vector3(pVoxelPos.x, pVoxelPos.y + voxelSize, pVoxelPos.z),

            //East
            new Vector3(pVoxelPos.x + voxelSize, pVoxelPos.y, pVoxelPos.z),

            //South
            new Vector3(pVoxelPos.x, pVoxelPos.y - voxelSize, pVoxelPos.z),

            //West
            new Vector3(pVoxelPos.x - voxelSize, pVoxelPos.y, pVoxelPos.z),

            //Center
            new Vector3(pVoxelPos.x, pVoxelPos.y, pVoxelPos.z + voxelSize),
            new Vector3(pVoxelPos.x, pVoxelPos.y, pVoxelPos.z - voxelSize),
        };

        return positions;
    }


    private List<int> calculateNeighbourVoxels(VoxelContainer pCurrentVoxel)
    {
        List<int> neighbourVoxels = new List<int>();

        Vector3[] directions = defineNeighbourVoxelPositions(pCurrentVoxel.Position);

        foreach (Vector3 voxelNeighbourDirection in directions)
        {
            VoxelContainer voxel = VoxelPositionHandler.GetVoxelFromWorldPos(voxelGridSaveFile.AllVoxels, voxelNeighbourDirection,
                transform.position, mapDimensionsFloat, voxelSize);

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