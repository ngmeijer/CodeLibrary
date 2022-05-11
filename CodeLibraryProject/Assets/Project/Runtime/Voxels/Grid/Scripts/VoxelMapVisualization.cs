using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(VoxelGridCalculator))]
public class VoxelMapVisualization : MonoBehaviour
{
    private VoxelGridCalculator voxelCalculator;
    private List<VoxelContainer> usedList;
    private float[] tempMapDimensions;
    private float currentVoxelSize;
    private Vector3 voxelVisualSize;
    private Vector3 startingVoxelPosition;
    private VoxelContainer currentVoxel;
    private List<VoxelContainer> selectedVoxelNeigbours = new List<VoxelContainer>();
    private List<int> selectedNeighboursIDs;

    [Header("General visuals")] [SerializeField]
    private bool showVisualization = true;

    [SerializeField] private bool showColliderVoxels;
    [SerializeField] private Color voxelWithColliderCol = new Color(1, 0, 0, 0.2f);

    [Header("Map borders")] [SerializeField]
    private bool showMapDimensions;

    [SerializeField] private Color borderColour = Color.blue;

    [Header("Neighbour voxels")] [SerializeField]
    private bool showNeighbourVoxels;

    [SerializeField] private int currentVoxelID;
    [SerializeField] private Color focusedVoxelColour = Color.yellow;
    [SerializeField] private Color neighbourVoxelsColour = Color.green;

    private void Start()
    {
        GetCalculatorReference();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (voxelCalculator == null) return;
        if (voxelCalculator.SaveFile == null) return;

        if (!showVisualization) return;
        if (voxelCalculator.SaveFile.AllVoxels.Count <= 0) return;

        currentVoxelID = Mathf.Clamp(currentVoxelID, 0, voxelCalculator.SaveFile.AllVoxels.Count);
        if (currentVoxelID < 1 || currentVoxelID > voxelCalculator.SaveFile.AllVoxels.Count)
        {
            Debug.Log(
                $"Exceeding voxel collection limits. Index must range from 1 to the max amount of voxels ({voxelCalculator.SaveFile.AllVoxels.Count}).");
        }
        else currentVoxel = voxelCalculator.SaveFile.AllVoxels[currentVoxelID];

        currentVoxelSize = voxelCalculator.SaveFile.VoxelSize;
        voxelVisualSize = new Vector3(currentVoxelSize, currentVoxelSize, currentVoxelSize);

        if (showColliderVoxels) handleVoxelVisualization();
        if (showMapDimensions) drawGridOuterBorders();
        if (showNeighbourVoxels) visualizeNeighbourVoxels();
    }

#endif

    private void GetCalculatorReference()
    {
        if (TryGetComponent(out VoxelGridCalculator calculator))
            voxelCalculator = calculator;
        Debug.Assert(voxelCalculator != null,
            $"{gameObject.name}'s calculatorReference is null. Check if component is attached.");
    }

    private void handleVoxelVisualization()
    {
        Gizmos.color = voxelWithColliderCol;
        foreach (KeyValuePair<int, VoxelContainer> voxel in voxelCalculator.SaveFile.ColliderVoxels)
        {
            Gizmos.DrawWireCube(voxel.Value.WorldPosition, voxelVisualSize);
        }
    }

    private void visualizeNeighbourVoxels()
    {
        if (currentVoxel == null)
            return;

        Gizmos.color = focusedVoxelColour;
        Gizmos.DrawCube(currentVoxel.WorldPosition, voxelVisualSize);
        Handles.Label(currentVoxel.WorldPosition,
            $"ID: {currentVoxel.ID}.\nGrid Position: {currentVoxel.GridPosition}");

        selectedNeighboursIDs = currentVoxel.NeighbourVoxelIDs;
        selectedVoxelNeigbours.Clear();

        foreach (int id in selectedNeighboursIDs)
        {
            if (id < 1 || id > voxelCalculator.SaveFile.AllVoxels.Count) continue;

            voxelCalculator.SaveFile.AllVoxels.TryGetValue(id, out VoxelContainer neighbour);
            selectedVoxelNeigbours.Add(neighbour);
        }

        foreach (VoxelContainer voxel in selectedVoxelNeigbours)
        {
            if (voxel == null) continue;
            Gizmos.color = neighbourVoxelsColour;
            Gizmos.DrawWireCube(voxel.WorldPosition, voxelVisualSize);
            Handles.Label(voxel.WorldPosition, $"ID: {voxel.ID}");
        }
    }

    private void drawGridOuterBorders()
    {
        tempMapDimensions = voxelCalculator.GetMapDimensions();
        Vector3 savedMapDimensions = voxelCalculator.SaveFile.MapDimensions;

        //Required for expectedVoxelCount
        float tempVoxelSize = voxelCalculator.GetVoxelSize();

        int tempVoxelCountX = (int) Math.Ceiling((tempMapDimensions[0] / tempVoxelSize));
        int tempVoxelCountY = (int) Math.Ceiling((tempMapDimensions[1] / tempVoxelSize));
        int tempVoxelCountZ = (int) Math.Ceiling((tempMapDimensions[2] / tempVoxelSize));

        int currentVoxelCountX = (int) Math.Ceiling((savedMapDimensions[0] / currentVoxelSize));
        int currentVoxelCountY = (int) Math.Ceiling((savedMapDimensions[1] / currentVoxelSize));
        int currentVoxelCountZ = (int) Math.Ceiling((savedMapDimensions[2] / currentVoxelSize));

        Vector3 gridStartPosition = startingVoxelPosition -
                                    new Vector3(currentVoxelSize / 2, currentVoxelSize / 2, currentVoxelSize / 2);
        Vector3 mapCenter = new Vector3(
            tempMapDimensions[0] / 2 - currentVoxelSize / 2, 
            tempMapDimensions[1] / 2 - currentVoxelSize / 2, 
            tempMapDimensions[2] / 2 - currentVoxelSize / 2);
        Gizmos.color = borderColour;
        Gizmos.DrawWireCube(mapCenter + transform.position,
            new Vector3(tempMapDimensions[0], tempMapDimensions[1], tempMapDimensions[2]));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gridStartPosition, gridStartPosition + (transform.right * tempMapDimensions[0]));
        Gizmos.DrawLine(gridStartPosition, gridStartPosition + (transform.up * tempMapDimensions[1]));
        Gizmos.DrawLine(gridStartPosition, gridStartPosition + (transform.forward * tempMapDimensions[2]));

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + (transform.right * tempMapDimensions[0]),
            $"Grid width: {tempMapDimensions[0]}m.\nCurrent voxel count: {currentVoxelCountX}. \nExpected voxel count: {tempVoxelCountX}");
        UnityEditor.Handles.Label(transform.position + (transform.up * tempMapDimensions[1]),
            $"Grid width: {tempMapDimensions[0]}m.\nCurrent voxel count: {currentVoxelCountY}. \nExpected voxel count: {tempVoxelCountY}");
        UnityEditor.Handles.Label(transform.position + (transform.forward * tempMapDimensions[2]),
            $"Grid width: {tempMapDimensions[0]}m.\nCurrent voxel count: {currentVoxelCountZ}. \nExpected voxel count: {tempVoxelCountZ}");
#endif
    }
}