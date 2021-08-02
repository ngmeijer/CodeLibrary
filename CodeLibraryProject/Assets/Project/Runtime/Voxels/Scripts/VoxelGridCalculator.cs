using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(VoxelMapVisualization), typeof(VoxelObstacleCalculator))]
public class VoxelGridCalculator : MonoBehaviour
{
    //Serialized fields
    [Header("Grid settings")] [Space(70)] [SerializeField] [Range(1, 1000)]
    private float sceneWidth = 50;

    [SerializeField] [Range(1, 500)] private float sceneHeight = 50;
    [SerializeField] [Range(1, 1000)] private float sceneDepth = 50;
    [ReadOnlyInspector] public Vector3 sceneDimensionsVector;

    [Space(15)] [SerializeField] [Range(0.1f, 50f)]
    private float voxelSize;

    //Public fields
    public VoxelGridData VoxelGridSaveFile;
    public string[] ColliderTagsToCompare;
    public Vector3 CalculatorPosition { get; private set; }

    [HideInInspector] public Vector3 GridCenterPosition;
    [HideInInspector] public float xAxisDelta;
    [HideInInspector] public float yAxisDelta;
    [HideInInspector] public float zAxisDelta;
    [HideInInspector] public Vector3 HandlesDelta;


    [Header("Grid properties")] 
    [ReadOnlyInspector] [SerializeField] private int totalExpectedVoxels;

     [ReadOnlyInspector] [SerializeField] private int totalCurrentVoxels = 0;


    [Space] 
    [ReadOnlyInspector] [SerializeField] private float calculationTimeMilliseconds;

    [ReadOnlyInspector] [SerializeField] private float calculationTimeSeconds;
    [ReadOnlyInspector] [SerializeField] private float calculationTimeMinutes;
    
    //Private fields
    private VoxelObstacleCalculator collisionChecker;
    private int voxelCountX;
    private int voxelCountY;
    private int voxelCountZ;
    private int octreeCellCountX;
    private int octreeCellCountY;
    private int octreeCellCountZ;

    private Vector3Int currentGridPosition;
    private Vector3Int neighbourGridPosition;
    private int convertedNeighbourID;
    private VoxelContainer neighbourVoxel;

    private void Awake() => collisionChecker = GetComponent<VoxelObstacleCalculator>();

    private void Update()
    {
        sceneWidth += HandlesDelta.x;
        sceneHeight += HandlesDelta.y;
        sceneDepth += HandlesDelta.z;
        
        HandlesDelta = Vector3.zero;

        voxelCountX = (int) Math.Ceiling(sceneWidth / voxelSize);
        voxelCountY = (int) Math.Ceiling(sceneHeight / voxelSize);
        voxelCountZ = (int) Math.Ceiling(sceneDepth / voxelSize);

        totalExpectedVoxels = voxelCountX * voxelCountY * voxelCountZ;
        
        sceneDimensionsVector = new Vector3(sceneWidth, sceneHeight, sceneDepth);
    }

    public void UpdateGridSizeThroughHandles()
    {
        GridCenterPosition = new Vector3(
            sceneDimensionsVector.x / 2 - voxelSize / 2, 
            sceneDimensionsVector.y / 2 - voxelSize / 2,
            sceneDimensionsVector.z / 2 - voxelSize / 2);
    }

    public void StartCalculationCoroutine()
    {
        StartCoroutine(RecalculateVoxelGrid());
    }

    public IEnumerator RecalculateVoxelGrid()
    {
        float startTime = Time.realtimeSinceStartup;
        
        if (VoxelGridSaveFile == null)
        {
            Debug.Log("VoxelGridData save file reference is null.");
            yield break;
        }

        if (collisionChecker == null)
        {
            Debug.Log("VoxelObstacleCalculator reference is null.");
            yield break;
        }

        VoxelCalculationConfirmPopup.Init();
        
        while (!VoxelCalculationConfirmPopup.HasClicked) yield return null;
        if (!VoxelCalculationConfirmPopup.HasContinued) yield break;
        
        ClearVoxelData();
        divideLevelIntoVoxels();
        collisionChecker.StartCollisionCheck(voxelSize);
        CalculateNeighboursAfterCollisionDetection();

        calculationTimeMilliseconds = (Time.realtimeSinceStartup - startTime) * 1000f;
        calculationTimeSeconds = (Time.realtimeSinceStartup - startTime);
        calculationTimeMinutes = calculationTimeSeconds / 60;
    }

    public void ClearVoxelData()
    {
        Console.Clear();
        VoxelGridSaveFile.AllVoxels.Clear();
        VoxelGridSaveFile.ColliderVoxels.Clear();
        VoxelGridSaveFile.TraversableVoxels.Clear();
        VoxelGridSaveFile.VoxelPositions.Clear();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearDirty(VoxelGridSaveFile);
#endif
    }

    private void divideLevelIntoVoxels()
    {
        CalculatorPosition = transform.position;
        VoxelGridSaveFile.VoxelSize = voxelSize;
        VoxelGridSaveFile.MapDimensions = new float[3] {sceneWidth + xAxisDelta, sceneHeight + yAxisDelta, sceneDepth + zAxisDelta};

        int voxelID = 1;

        ProgressBar.MaxVoxelIndex = totalExpectedVoxels;

        for (int x = 0; x < voxelCountX; x++)
            for (int y = 0; y < voxelCountY; y++)
                for (int z = 0; z < voxelCountZ; z++)
                {
                    VoxelContainer voxel = new VoxelContainer
                    {
                        WorldPosition = new Vector3(CalculatorPosition.x + (voxelSize * x), CalculatorPosition.y + (voxelSize * y),
                            CalculatorPosition.z + (voxelSize * z)),
                        ID = voxelID,
                        GridPosition = new Vector3Int(x, y, z)
                    };
                    ProgressBar.ShowVoxelCreateProgress(voxelID);

                    VoxelGridSaveFile.AllVoxels.Add(voxel.ID, voxel);
                    VoxelGridSaveFile.TraversableVoxels.Add(voxel.ID, voxel);
                    VoxelGridSaveFile.VoxelPositions.Add(voxel.GridPosition, voxel.ID);

                    voxelID++;
                }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(VoxelGridSaveFile);

        totalCurrentVoxels = VoxelGridSaveFile.AllVoxels.Count;
        ProgressBar.ShowVoxelCreateProgress(voxelID);
#endif
    }

    public void CalculateNeighboursAfterCollisionDetection()
    {
        int currentVoxelIndex = 1;
        ProgressBar.MaxVoxelIndex = VoxelGridSaveFile.AllVoxels.Count;
        foreach (KeyValuePair<int, VoxelContainer> voxel in VoxelGridSaveFile.AllVoxels)
        {
            currentVoxelIndex++;
            VoxelContainer currentVoxel = voxel.Value;
            ProgressBar.ShowVoxelNeighbourProgress(currentVoxelIndex);
            List<int> neighbourVoxelIDs = calculateNeighbourVoxels(currentVoxel);
            currentVoxel.NeighbourVoxelIDs = neighbourVoxelIDs;
        }

        ProgressBar.ShowVoxelNeighbourProgress(ProgressBar.MaxVoxelIndex);
    }


    private List<int> calculateNeighbourVoxels(VoxelContainer pCurrentVoxel)
    {
        List<int> neighbourVoxels = new List<int>();
        currentGridPosition = pCurrentVoxel.GridPosition;

        {
            //Top
            neighbourGridPosition = currentGridPosition;
            neighbourGridPosition.y += 1;
            if (neighbourGridPosition.y <= voxelCountY - 1)
                calculateSingleNeighbourVoxelID(neighbourVoxels, neighbourGridPosition);

            //Bottom
            neighbourGridPosition = currentGridPosition;
            neighbourGridPosition.y -= 1;
            if (neighbourGridPosition.y >= 0)
                calculateSingleNeighbourVoxelID(neighbourVoxels, neighbourGridPosition);
        }

        {
            //Left
            neighbourGridPosition = currentGridPosition;
            neighbourGridPosition.x -= 1;
            if (neighbourGridPosition.x >= 0)
                calculateSingleNeighbourVoxelID(neighbourVoxels, neighbourGridPosition);

            //Right
            neighbourGridPosition = currentGridPosition;
            neighbourGridPosition.x += 1;
            if (neighbourGridPosition.x <= voxelCountX - 1)
                calculateSingleNeighbourVoxelID(neighbourVoxels, neighbourGridPosition);
        }

        {
            //Center front
            neighbourGridPosition = currentGridPosition;
            neighbourGridPosition.z -= 1;
            if (neighbourGridPosition.z >= 0)
                calculateSingleNeighbourVoxelID(neighbourVoxels, neighbourGridPosition);

            //Center back
            neighbourGridPosition = currentGridPosition;
            neighbourGridPosition.z += 1;
            if (neighbourGridPosition.z <= voxelCountZ - 1)
                calculateSingleNeighbourVoxelID(neighbourVoxels, neighbourGridPosition);
        }

        return neighbourVoxels;
    }

    private void calculateSingleNeighbourVoxelID(List<int> pNeighbourVoxelIDs,
        Vector3Int pNeighbourPosition)
    {
        VoxelGridSaveFile.VoxelPositions.TryGetValue(pNeighbourPosition, out int neighbourID);
        if (neighbourID != 0) pNeighbourVoxelIDs.Add(neighbourID);
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