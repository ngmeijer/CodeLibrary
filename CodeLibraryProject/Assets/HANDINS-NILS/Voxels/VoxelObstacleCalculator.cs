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

    [ReadOnlyInspector] [SerializeField] private int octreeIterations;

    private void Awake()
    {
        calculator = GetComponent<VoxelGridCalculator>();
    }

    public void StartCollisionCheck(float pVoxelSize)
    {
        if (calculator == null)
        {
            Debug.LogError("VoxelGridCalculator reference in VoxelCollisionOverlapCheck is null.");
            return;
        }

        string[] tags = calculator.ColliderTagsToCompare;
        Dictionary<int, VoxelContainer> allVoxels = calculator.voxelGridSaveFile.AllVoxels;
        SerializableDictionary<int, VoxelContainer> colliderVoxels = calculator.voxelGridSaveFile.ColliderVoxels;

        Vector3 colliderSize = new Vector3(pVoxelSize / 2, pVoxelSize / 2, pVoxelSize / 2);

        ProgressBar.MaxVoxelIndex = allVoxels.Count - 1;
        for (int voxelIndex = 0; voxelIndex < allVoxels.Count; voxelIndex++)
        {
            ProgressBar.ShowVoxelCollisionProgress(voxelIndex);
            currentVoxel = allVoxels[voxelIndex];

            Collider[] allColliders = Physics.OverlapBox(currentVoxel.Position,
                colliderSize);

            foreach (Collider foundCollider in allColliders)
            {
                if (tagsToCompare.Contains(foundCollider.tag))
                    continue;

                currentVoxel.IsTraversable = false;
                if(!colliderVoxels.ContainsKey(currentVoxel.ID))
                    colliderVoxels.Add(currentVoxel.ID, currentVoxel);
                calculator.voxelGridSaveFile.TraversableVoxels.Remove(currentVoxel.ID);
            }
        }

        calculator.CalculateNeighboursAfterCollisionDetection();

        ProgressBar.HasFinishedProcess = true;
        ProgressBar.ShowVoxelCollisionProgress(allVoxels.Count - 1);
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