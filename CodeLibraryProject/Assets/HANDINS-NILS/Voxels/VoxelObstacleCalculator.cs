using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class VoxelObstacleCalculator : MonoBehaviour
{
    private VoxelContainer currentVoxel;
    private VoxelGridCalculator calculator;

    [SerializeField] private string[] tagsToCompare;
    [ReadOnlyInspector] [SerializeField] [Tooltip("Time in milliseconds")] private float calculationTimeMilliseconds;
    [ReadOnlyInspector] [SerializeField] [Tooltip("Time in seconds")] private float calculationTimeSeconds;
    [ReadOnlyInspector] [SerializeField] private int octreeIterations;

    private void Awake()
    {
        calculator = GetComponent<VoxelGridCalculator>();
    }

    public void StartCollisionCheck()
    {
        if (calculator == null)
        {
            Debug.LogError("VoxelGridCalculator reference in VoxelCollisionOverlapCheck is null.");
            return;
        }

        float startTime = Time.realtimeSinceStartup;
        
        Dictionary<int, VoxelContainer> allVoxels = calculator.voxelGridSaveFile.AllVoxels;

        float voxelSize = calculator.VoxelSize;
        float meshColliderAccuracy = calculator.meshColliderAccuracy;
        float colliderSizeAxis = voxelSize / meshColliderAccuracy;

        Vector3 colliderSize = new Vector3(colliderSizeAxis, colliderSizeAxis, colliderSizeAxis);

        for (int voxelIndex = 0; voxelIndex < allVoxels.Count; voxelIndex++)
        {
            currentVoxel = allVoxels[voxelIndex];
            if (!currentVoxel.IsTraversable)
                continue;

            Collider[] allColliders = Physics.OverlapBox(currentVoxel.Position,
                colliderSize);

            for (int colliderIndex = 0; colliderIndex < allColliders.Length; colliderIndex++)
            {
                if (tagsToCompare.Contains(allColliders[colliderIndex].tag))
                    continue;

                currentVoxel.IsTraversable = false;
                if(!calculator.voxelGridSaveFile.ColliderVoxels.ContainsKey(currentVoxel.ID))
                    calculator.voxelGridSaveFile.ColliderVoxels.Add(currentVoxel.ID, currentVoxel);
                calculator.voxelGridSaveFile.TraversableVoxels.Remove(currentVoxel.ID);
            }
        }

        calculator.CalculateNeighboursAfterCollisionDetection();
        calculationTimeMilliseconds = (Time.realtimeSinceStartup - startTime) * 1000f;
        calculationTimeSeconds = (Time.realtimeSinceStartup - startTime);
    }

    private void handleOctreeDivisionIteration()
    {
        //Get map dimensions
        float[] mapDimensions = calculator.GetMapDimensions();
        //Create collider size
        Vector3 startSize = new Vector3(mapDimensions[0], mapDimensions[1], mapDimensions[2]);
        
        //Calculate "displacement" counts (how many times should the Physics.Overlap function be repeated
        //For loop the Physics.Overlap with given count & size.
    }

    private void calculateIterationCount()
    {
        
    }
}