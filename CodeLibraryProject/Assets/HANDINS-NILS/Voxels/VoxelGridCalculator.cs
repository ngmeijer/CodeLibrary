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
    //Serialized fields
    [Header("Grid settings")] [Space(70)]
    [SerializeField] [Range(1, 1000)] private float sceneWidth = 50;
    [SerializeField] [Range(1, 500)] private float sceneHeight = 50;
    [SerializeField] [Range(1, 1000)] private float sceneDepth = 50;
    [ReadOnlyInspector] [SerializeField] private Vector3 sceneDimensionsVector;

    [Space(15)] 
    [SerializeField] [Range(1f, 50f)] private float voxelSize;

    //Public fields
    public VoxelGridData VoxelGridSaveFile;
    public string[] ColliderTagsToCompare;

    //Private fields
    private Vector3 pos;
    private VoxelObstacleCalculator collisionChecker;
    private int voxelCountX;
    private int voxelCountY;
    private int voxelCountZ;
    private int octreeCellCountX;
    private int octreeCellCountY;
    private int octreeCellCountZ;

    private float[] mapDimensionsFloat;

    [Header("Grid properties")]
    [ReadOnlyInspector] [SerializeField] private int totalExpectedVoxels = 0;
    [ReadOnlyInspector] [SerializeField] private int totalCurrentVoxels = 0;
    [Space]
    [ReadOnlyInspector] [SerializeField] private float calculationTimeMilliseconds;
    [ReadOnlyInspector] [SerializeField] private float calculationTimeSeconds;

    private void Awake() => collisionChecker = GetComponent<VoxelObstacleCalculator>();
    
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

        if (VoxelGridSaveFile == null)
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
        CalculateNeighboursAfterCollisionDetection();

        calculationTimeMilliseconds = (Time.realtimeSinceStartup - startTime) * 1000f;
        calculationTimeSeconds = (Time.realtimeSinceStartup - startTime);
    }

    public void ClearVoxelData()
    {
        Console.Clear();
        VoxelGridSaveFile.AllVoxels.Clear();
        VoxelGridSaveFile.ColliderVoxels.Clear();
        VoxelGridSaveFile.TraversableVoxels.Clear();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearDirty(VoxelGridSaveFile);
#endif
    }

    private void divideLevelIntoVoxels()
    {
        pos = transform.position;
        VoxelGridSaveFile.VoxelSize = voxelSize;
        VoxelGridSaveFile.MapDimensions = new float[3] {sceneWidth, sceneHeight, sceneDepth};

        int voxelID = 0;

        ProgressBar.MaxVoxelIndex = totalExpectedVoxels - 1;
        
        for (int x = 0; x < voxelCountX; x++)
            for (int y = 0; y < voxelCountY; y++)
                for (int z = 0; z < voxelCountZ; z++)
                {
                    VoxelContainer voxel = new VoxelContainer
                    {
                        WorldPosition = new Vector3(pos.x + (voxelSize * x), pos.y + (voxelSize * y),
                            pos.z + (voxelSize * z)),
                        ID = voxelID,
                        GridPosition = new Vector3(x,y,z)
                    };
                    ProgressBar.ShowVoxelCreateProgress(voxelID);

                    VoxelGridSaveFile.AllVoxels.Add(voxel.ID, voxel);
                    VoxelGridSaveFile.TraversableVoxels.Add(voxel.ID, voxel);

                    voxelID++;
                }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(VoxelGridSaveFile);

        totalCurrentVoxels = VoxelGridSaveFile.AllVoxels.Count;
        
        ProgressBar.HasFinishedProcess = true;
        ProgressBar.ShowVoxelCreateProgress(voxelID);
#endif
    }

    /// <summary>
    /// This function needs to be optimized.
    /// </summary>
    public void CalculateNeighboursAfterCollisionDetection()
    {
        int currentVoxelIndex = 0;
        ProgressBar.MaxVoxelIndex = VoxelGridSaveFile.TraversableVoxels.Count - 1;
        mapDimensionsFloat = GetMapDimensions();
        foreach (KeyValuePair<int, VoxelContainer> voxel in VoxelGridSaveFile.TraversableVoxels)
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


    private List<int> calculateNeighbourVoxels(VoxelContainer pCurrentVoxel)
    {
        List<int> neighbourVoxels = new List<int>();
        Vector3 currentGridPosition = pCurrentVoxel.GridPosition;
        Vector3 neighbourGridPosition;
        int convertedNeighbourID;
        
        //North
        neighbourGridPosition = currentGridPosition;
        neighbourGridPosition.y += 1;
        convertedNeighbourID = (int)(neighbourGridPosition.x + neighbourGridPosition.y + neighbourGridPosition.z);
        neighbourVoxels.Add(convertedNeighbourID);

        //East
        neighbourGridPosition = currentGridPosition;
        neighbourGridPosition.x -= 1;
        convertedNeighbourID = (int)(neighbourGridPosition.x + neighbourGridPosition.y + neighbourGridPosition.z);
        neighbourVoxels.Add(convertedNeighbourID);

        //South
        neighbourGridPosition = currentGridPosition;
        neighbourGridPosition.y -= 1;
        convertedNeighbourID = (int)(neighbourGridPosition.x + neighbourGridPosition.y + neighbourGridPosition.z);
        neighbourVoxels.Add(convertedNeighbourID);

        //West
        neighbourGridPosition = currentGridPosition;
        neighbourGridPosition.x += 1;
        convertedNeighbourID = (int)(neighbourGridPosition.x + neighbourGridPosition.y + neighbourGridPosition.z);
        neighbourVoxels.Add(convertedNeighbourID);

        //Center front
        neighbourGridPosition = currentGridPosition;
        neighbourGridPosition.z += 1;
        convertedNeighbourID = (int)(neighbourGridPosition.x + neighbourGridPosition.y + neighbourGridPosition.z);
        neighbourVoxels.Add(convertedNeighbourID);
        
        //Center back
        neighbourGridPosition = currentGridPosition;
        neighbourGridPosition.z -= 1;
        convertedNeighbourID = (int)(neighbourGridPosition.x + neighbourGridPosition.y + neighbourGridPosition.z);
        neighbourVoxels.Add(convertedNeighbourID);

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