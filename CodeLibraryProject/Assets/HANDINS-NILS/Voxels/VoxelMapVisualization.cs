using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(VoxelGridCalculator))]
public class VoxelMapVisualization : MonoBehaviour
{
    private VoxelGridCalculator voxelCalculator;
    private List<VoxelContainer> usedList;
    private float[] mapDimensions;
    private float voxelSize;
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

        voxelSize = saveFile.VoxelSize;
        voxelVisualSize = new Vector3(voxelSize, voxelSize, voxelSize);

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
        mapDimensions = voxelCalculator.GetMapDimensions();

        Vector3 gridStartPosition = startingVoxelPosition - new Vector3(voxelSize / 2, voxelSize / 2, voxelSize / 2);
        Vector3 mapCenter = new Vector3(mapDimensions[0] / 2 - voxelSize / 2, mapDimensions[1] / 2
                                                                              - voxelSize / 2,
            mapDimensions[2] / 2 - voxelSize / 2);
        Gizmos.color = borderColour;
        Gizmos.DrawWireCube(mapCenter + transform.position,
            new Vector3(mapDimensions[0], mapDimensions[1], mapDimensions[2]));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gridStartPosition, gridStartPosition + (transform.right * mapDimensions[0]));
        Gizmos.DrawLine(gridStartPosition, gridStartPosition + (transform.up * mapDimensions[1]));
        Gizmos.DrawLine(gridStartPosition, gridStartPosition + (transform.forward * mapDimensions[2]));

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + (transform.right * mapDimensions[0]),
            $"{$"Grid width: {mapDimensions[0]}"}");
        UnityEditor.Handles.Label(transform.position + (transform.up * mapDimensions[1]),
            $"{$"Grid height: {mapDimensions[1]}"}");
        UnityEditor.Handles.Label(transform.position + (transform.forward * mapDimensions[2]),
            $"{$"Grid depth: {mapDimensions[2]}"}");
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