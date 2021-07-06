using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class VoxelCollisionOverlapCheck : MonoBehaviour
{
    private VoxelContainer currentVoxel;
    private VoxelGridCalculator calculator;

    [SerializeField] private string[] tagsToCompare;
    [ReadOnlyInspector] [SerializeField] private float calculationTimeTaken;

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

        calculationTimeTaken = Time.realtimeSinceStartup - startTime * 1000f;
    }
}