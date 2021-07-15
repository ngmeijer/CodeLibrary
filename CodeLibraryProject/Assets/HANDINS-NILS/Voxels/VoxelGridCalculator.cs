using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
[RequireComponent(typeof(VoxelMapVisualization), typeof(VoxelObstacleCalculator))]
public class VoxelGridCalculator : MonoBehaviour
{
    //Serialized fields
    [Header("Grid settings")] [Space(70)] [SerializeField] [Range(1, 1000)]
    private float sceneWidth = 50;

    [SerializeField] [Range(1, 500)] private float sceneHeight = 50;
    [SerializeField] [Range(1, 1000)] private float sceneDepth = 50;
    [ReadOnlyInspector] [SerializeField] private Vector3 sceneDimensionsVector;

    [Space(15)] [SerializeField] [Range(1f, 50f)]
    private float voxelSize;

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

    private Vector3Int currentGridPosition;
    private Vector3Int neighbourGridPosition;
    private int convertedNeighbourID;
    private VoxelContainer neighbourVoxel;

    [Header("Grid properties")] [ReadOnlyInspector] [SerializeField]
    private int totalExpectedVoxels = 0;

    [ReadOnlyInspector] [SerializeField] private int totalCurrentVoxels = 0;

    [Space] [ReadOnlyInspector] [SerializeField]
    private float calculationTimeMilliseconds;

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
                        GridPosition = new Vector3Int(x, y, z)
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

    public void CalculateNeighboursAfterCollisionDetection()
    {
        int currentVoxelIndex = 0;
        ProgressBar.MaxVoxelIndex = VoxelGridSaveFile.TraversableVoxels.Count - 1;
        foreach (KeyValuePair<int, VoxelContainer> voxel in VoxelGridSaveFile.AllVoxels)
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
        currentGridPosition = pCurrentVoxel.GridPosition;

        //North
        neighbourGridPosition.y += 1;
        convertedNeighbourID = neighbourGridPosition.x * 100 + neighbourGridPosition.y * 10 + neighbourGridPosition.z;
        if (neighbourGridPosition.x > 0 && neighbourGridPosition.y > 0 && neighbourGridPosition.z > 0)
        {
            neighbourVoxels.Add(convertedNeighbourID);
            VoxelGridSaveFile.AllVoxels.TryGetValue(convertedNeighbourID, out neighbourVoxel);
            if (neighbourVoxel != null)
                pCurrentVoxel.neighbourData.neighbourVoxels.Add(convertedNeighbourID, neighbourVoxel);
        }

        //East
        neighbourGridPosition = currentGridPosition;
        neighbourGridPosition.x -= 1;
        convertedNeighbourID = neighbourGridPosition.x * 100 + neighbourGridPosition.y * 10 + neighbourGridPosition.z;
        if (neighbourGridPosition.x > 0 && neighbourGridPosition.y > 0 && neighbourGridPosition.z > 0)
        {
            neighbourVoxels.Add(convertedNeighbourID);
            VoxelGridSaveFile.AllVoxels.TryGetValue(convertedNeighbourID, out neighbourVoxel);
            if (neighbourVoxel != null)
                pCurrentVoxel.neighbourData.neighbourVoxels.Add(convertedNeighbourID, neighbourVoxel);
        }
        //South
        neighbourGridPosition = currentGridPosition;
        neighbourGridPosition.y -= 1;
        convertedNeighbourID = neighbourGridPosition.x * 100 + neighbourGridPosition.y * 10 + neighbourGridPosition.z;
        if (neighbourGridPosition.x > 0 && neighbourGridPosition.y > 0 && neighbourGridPosition.z > 0)
        {
            neighbourVoxels.Add(convertedNeighbourID);
            VoxelGridSaveFile.AllVoxels.TryGetValue(convertedNeighbourID, out neighbourVoxel);
            if (neighbourVoxel != null)
                pCurrentVoxel.neighbourData.neighbourVoxels.Add(convertedNeighbourID, neighbourVoxel);
        }
        
        // //West
        neighbourGridPosition = currentGridPosition;
        neighbourGridPosition.x += 1;
        convertedNeighbourID = neighbourGridPosition.x * 100 + neighbourGridPosition.y * 10 + neighbourGridPosition.z;
        if (neighbourGridPosition.x > 0 && neighbourGridPosition.y > 0 && neighbourGridPosition.z > 0)
        {
            neighbourVoxels.Add(convertedNeighbourID);
            VoxelGridSaveFile.AllVoxels.TryGetValue(convertedNeighbourID, out neighbourVoxel);
            if (neighbourVoxel != null)
                pCurrentVoxel.neighbourData.neighbourVoxels.Add(convertedNeighbourID, neighbourVoxel);
        }
        
        //Center front
        neighbourGridPosition = currentGridPosition;
        neighbourGridPosition.z -= 1;
        convertedNeighbourID = neighbourGridPosition.x * 100 + neighbourGridPosition.y * 10 + neighbourGridPosition.z;
        if (neighbourGridPosition.x > 0 && neighbourGridPosition.y > 0 && neighbourGridPosition.z > 0)
        {
            neighbourVoxels.Add(convertedNeighbourID);
            VoxelGridSaveFile.AllVoxels.TryGetValue(convertedNeighbourID, out neighbourVoxel);
            if (neighbourVoxel != null)
                pCurrentVoxel.neighbourData.neighbourVoxels.Add(convertedNeighbourID, neighbourVoxel);
        }
        
        //Center back
        neighbourGridPosition = currentGridPosition;
        neighbourGridPosition.z += 1;
        convertedNeighbourID = neighbourGridPosition.x * 100 + neighbourGridPosition.y * 10 + neighbourGridPosition.z;
        if (neighbourGridPosition.x > 0 && neighbourGridPosition.y > 0 && neighbourGridPosition.z > 0)
        {
            neighbourVoxels.Add(convertedNeighbourID);
            VoxelGridSaveFile.AllVoxels.TryGetValue(convertedNeighbourID, out neighbourVoxel);
            if (neighbourVoxel != null && !pCurrentVoxel.neighbourData.neighbourVoxels.ContainsKey(convertedNeighbourID))
                pCurrentVoxel.neighbourData.neighbourVoxels.Add(convertedNeighbourID, neighbourVoxel);
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