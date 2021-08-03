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
        Dictionary<int, VoxelContainer> allVoxels = calculator.SaveFile.AllVoxels;
        SerializableDictionary<int, VoxelContainer> colliderVoxels = calculator.SaveFile.ColliderVoxels;

        Vector3 colliderSize = new Vector3(pVoxelSize / 2, pVoxelSize / 2, pVoxelSize / 2);

        ProgressBar.MaxVoxelIndex = allVoxels.Count;
        for (int voxelIndex = 1; voxelIndex < allVoxels.Count; voxelIndex++)
        {
            ProgressBar.ShowVoxelCollisionProgress(voxelIndex);
            currentVoxel = allVoxels[voxelIndex];

            Collider[] allColliders = Physics.OverlapBox(currentVoxel.WorldPosition,
                colliderSize);

            foreach (Collider foundCollider in allColliders)
            {
                if (tags.Contains(foundCollider.tag))
                    continue;

                currentVoxel.IsTraversable = false;
                if(!colliderVoxels.ContainsKey(currentVoxel.ID))
                    colliderVoxels.Add(currentVoxel.ID, currentVoxel);
                calculator.SaveFile.TraversableVoxels.Remove(currentVoxel.ID);
            }
        }
        
        ProgressBar.ShowVoxelCollisionProgress(allVoxels.Count);
    }
}