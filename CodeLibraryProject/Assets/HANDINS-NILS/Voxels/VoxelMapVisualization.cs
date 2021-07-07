using System;
using System.Collections.Generic;
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

    [Header("Octree visuals")] [SerializeField]
    private bool showOctreeDivisions;

    [SerializeField] private int octreeDivisionDepth;
    [SerializeField] private Color iterationContainerColour;

    private void Start()
    {
        GetCalculatorReference();
    }

    private void OnDrawGizmos()
    {
        if (!showVisualization) return;

        currentVoxelSize = saveFile.VoxelSize;
        voxelVisualSize = new Vector3(currentVoxelSize, currentVoxelSize, currentVoxelSize);

        if (showColliderVoxels) drawColliderVoxels();

        if (voxelCalculator == null) return;
        if (showMapDimensions) drawGridOuterBorders();
        if (showVoxelSize) drawVoxelSample();
        if (showOctreeDivisions) drawOctreeDivisions();
    }

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
            Gizmos.DrawWireCube(voxel.Value.Position, voxelVisualSize);
        }
    }

    private void drawGridOuterBorders()
    {
        tempMapDimensions = voxelCalculator.GetMapDimensions();
        float[] currentMapDimensions = saveFile.MapDimensions;

        float tempVoxelSize = voxelCalculator.GetVoxelSize();

        int tempVoxelCountX = (int)Math.Ceiling((tempMapDimensions[0] / tempVoxelSize));
        int tempVoxelCountY = (int)Math.Ceiling((tempMapDimensions[1] / tempVoxelSize));
        int tempVoxelCountZ = (int)Math.Ceiling((tempMapDimensions[2] / tempVoxelSize));
        
        int currentVoxelCountX = (int)Math.Ceiling((currentMapDimensions[0] / currentVoxelSize));
        int currentVoxelCountY = (int)Math.Ceiling((currentMapDimensions[1] / currentVoxelSize));
        int currentVoxelCountZ = (int)Math.Ceiling((currentMapDimensions[2] / currentVoxelSize));

        Vector3 gridStartPosition = startingVoxelPosition - new Vector3(currentVoxelSize / 2, currentVoxelSize / 2, currentVoxelSize / 2);
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
        if (voxelCalculator.voxelGridSaveFile.AllVoxels.Count <= 0) return;

        startingVoxelPosition = saveFile.AllVoxels[0].Position;
        Gizmos.color = voxelColour;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(startingVoxelPosition, "Grid's starting voxel");
#endif
        Gizmos.DrawCube(startingVoxelPosition, voxelVisualSize);
    }

    private void drawOctreeDivisions()
    {
        if (voxelCalculator.voxelGridSaveFile.OctreeDivisions.Count <= 0) return;
        SerializableDictionary<int, OctreeContainer> collection =
            voxelCalculator.voxelGridSaveFile.OctreeDivisions[octreeDivisionDepth];
        if (collection.Count <= 0) return;
        if (octreeDivisionDepth >= collection.Count) return;

        for (int i = 0; i < collection.Count; i++)
        {
            Gizmos.DrawCube(collection[i].Position, collection[i].OctreeNodeSize);
        }
    }
}