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

    [SerializeField] private VoxelGridData saveFile;

    [Header("General visuals")] 
    [SerializeField] private bool showVisualization = true;

    [SerializeField] private bool showAllVoxels;

    [SerializeField]
    private bool showColliderVoxels = true;

    [SerializeField] private Color voxelWithColliderCol = new Color(1, 0, 0, 0.2f);
    [SerializeField] private Color voxelNoColliderCol = new Color(0, 1, 0, 0.1f);

    [Header("Voxel size")] 
    [SerializeField] private bool showVoxelSize;

    [SerializeField] private Color voxelColour = Color.yellow;

    [Header("Map borders")] 
    [SerializeField] private bool showMapDimensions;

    [SerializeField] private Color borderColour = Color.blue;

    private void Start()
    {
        GetCalculatorReference();
    }

    private void OnDrawGizmos()
    {
        if (!showVisualization) return;
        
        voxelSize = saveFile.VoxelSize;
        voxelVisualSize = new Vector3(voxelSize, voxelSize, voxelSize);

        if (showAllVoxels) drawAllVoxels();
        if (showColliderVoxels && !showAllVoxels) drawColliderVoxels();
        if (showMapDimensions) drawGridOuterBorders();
        if (showVoxelSize) drawVoxelSample();
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

    private void drawAllVoxels()
    {
        foreach (KeyValuePair<int, VoxelContainer> voxel in saveFile.AllVoxels)
        {
            //TODO:
            //See if this can be optimized. Setting a colour for every single voxel (I have reached numbers of 200.000 voxels) is not very efficient,
            //Instead, set the default colour to "voxelNoColliderCol", and only change if a collider is detected. This should save some milliseconds.
            if (!voxel.Value.IsTraversable)
            {
                voxel.Value.ActiveColour = voxelWithColliderCol;
            }
            else voxel.Value.ActiveColour = voxelNoColliderCol;

            Gizmos.color = voxel.Value.ActiveColour;
            Gizmos.DrawWireCube(voxel.Value.Position, voxelVisualSize);
        }
    }

    private void drawGridOuterBorders()
    {
        mapDimensions = voxelCalculator.GetMapDimensions();

        Vector3 mapCenter = new Vector3(mapDimensions[0] / 2 - voxelSize / 2, mapDimensions[1] / 2
                                                                              - voxelSize / 2,
            mapDimensions[2] / 2 - voxelSize / 2);
        Gizmos.color = borderColour;
        Gizmos.DrawWireCube(mapCenter + transform.position,
            new Vector3(mapDimensions[0], mapDimensions[1], mapDimensions[2]));
    }

    private void drawVoxelSample()
    {
        if (voxelCalculator.voxelGridSaveFile.AllVoxels.Count > 0)
        {
            Vector3 startingVoxel = saveFile.AllVoxels[0].Position;

            Gizmos.color = voxelColour;
#if UNITY_EDITOR
            UnityEditor.Handles.Label(startingVoxel, "Grid's starting voxel");
#endif

            Gizmos.DrawCube(startingVoxel, voxelVisualSize);
        }
    }
}