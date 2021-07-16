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
    private List<VoxelContainer> selectedVoxelNeigbours;
    private List<int> selectedNeighboursIDs;

    [SerializeField] private VoxelGridData saveFile;

    [Header("General visuals")] [SerializeField]
    private bool showVisualization = true;

    [SerializeField] private bool showColliderVoxels = true;
    [SerializeField] private Color voxelWithColliderCol = new Color(1, 0, 0, 0.2f);

    [Header("Voxel size")] [SerializeField]
    private bool showVoxelSize;

    [SerializeField] private Color voxelColour = Color.yellow;

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
        if (voxelCalculator == null)
        {
            GetCalculatorReference();
            Debug.LogError("VoxelGridCalculator reference in VoxelMapVisualization is null.");
            return;
        }

        if (voxelCalculator.VoxelGridSaveFile == null)
        {
            Debug.LogError("VoxelGridData save file reference in VoxelGridCalculator is null.");
            return;
        }

        if (!showVisualization) return;
        if (saveFile.AllVoxels.Count <= 0) return;


        currentVoxelID = Mathf.Clamp(currentVoxelID, 0, saveFile.AllVoxels.Count - 1);
        currentVoxel = saveFile.AllVoxels[currentVoxelID];
        currentVoxelSize = saveFile.VoxelSize;
        voxelVisualSize = new Vector3(currentVoxelSize, currentVoxelSize, currentVoxelSize);

        if (showColliderVoxels) drawColliderVoxels();
        if (voxelCalculator == null) return;
        if (showMapDimensions) drawGridOuterBorders();
        if (showVoxelSize) drawVoxelSample();
        if (showNeighbourVoxels) visualizeNeighbourVoxels();
    }

#endif

    private void GetCalculatorReference()
    {
        if (TryGetComponent(out VoxelGridCalculator pCalculator))
            voxelCalculator = pCalculator;
        Debug.Assert(voxelCalculator != null,
            $"{gameObject.name}'s calculatorReference is null. Check if component is attached.");
    }

    private void drawColliderVoxels()
    {
        foreach (KeyValuePair<int, VoxelContainer> voxel in saveFile.ColliderVoxels)
        {
            //TODO:
            //See if this can be optimized. Setting a colour for every single voxel (I have reached numbers of 200.000 voxels) is not very efficient,
            //Instead, set the default colour to "voxelNoColliderCol", and only change if a collider is detected. This should save some milliseconds.
            voxel.Value.ActiveColour = voxelWithColliderCol;

            Gizmos.color = voxel.Value.ActiveColour;
            Gizmos.DrawWireCube(voxel.Value.WorldPosition, voxelVisualSize);
        }
    }

    private void visualizeNeighbourVoxels()
    {
        if (currentVoxel == null)
            return;

        Gizmos.color = focusedVoxelColour;
        Gizmos.DrawCube(currentVoxel.WorldPosition, voxelVisualSize);
        Handles.Label(currentVoxel.WorldPosition, $"ID: {currentVoxel.ID}");

        selectedNeighboursIDs = currentVoxel.NeighbourVoxelIDs;
        selectedVoxelNeigbours.Clear();

        foreach (int id in selectedNeighboursIDs)
        {
            if (currentVoxelID < 0 || currentVoxelID > saveFile.AllVoxels.Count - 1)
            {
                Debug.Log("Exceeding voxel collection limits. Index must range from 0 to the max amount of voxels - 1.");
                break;
            }
            saveFile.AllVoxels.TryGetValue(id, out VoxelContainer neighbour);
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
        float[] currentMapDimensions = saveFile.MapDimensions;

        //Required for expectedVoxelCount
        float tempVoxelSize = voxelCalculator.GetVoxelSize();

        int tempVoxelCountX = (int) Math.Ceiling((tempMapDimensions[0] / tempVoxelSize));
        int tempVoxelCountY = (int) Math.Ceiling((tempMapDimensions[1] / tempVoxelSize));
        int tempVoxelCountZ = (int) Math.Ceiling((tempMapDimensions[2] / tempVoxelSize));

        int currentVoxelCountX = (int) Math.Ceiling((currentMapDimensions[0] / currentVoxelSize));
        int currentVoxelCountY = (int) Math.Ceiling((currentMapDimensions[1] / currentVoxelSize));
        int currentVoxelCountZ = (int) Math.Ceiling((currentMapDimensions[2] / currentVoxelSize));

        Vector3 gridStartPosition = startingVoxelPosition -
                                    new Vector3(currentVoxelSize / 2, currentVoxelSize / 2, currentVoxelSize / 2);
        Vector3 mapCenter = new Vector3(tempMapDimensions[0] / 2 - currentVoxelSize / 2, tempMapDimensions[1] / 2
            - currentVoxelSize / 2,
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

    private void drawVoxelSample()
    {
        if (saveFile.AllVoxels.Count <= 0) return;

        startingVoxelPosition = saveFile.AllVoxels[0].WorldPosition;
        Gizmos.color = voxelColour;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(startingVoxelPosition, "Grid's starting voxel");
#endif
        Gizmos.DrawCube(startingVoxelPosition, voxelVisualSize);
    }
}