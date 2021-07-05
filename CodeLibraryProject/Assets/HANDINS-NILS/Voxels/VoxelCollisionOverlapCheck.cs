using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Event_OnVoxelCollisionFinished : UnityEvent
{
}

[ExecuteInEditMode]
public class VoxelCollisionOverlapCheck : MonoBehaviour
{
    private VoxelContainer currentVoxel;

    [SerializeField] private Event_OnVoxelCollisionFinished onColliderDetectionFinished;
    [SerializeField] private VoxelGridCalculator calculator;
    [SerializeField] private string[] tagsToCompare;

    private void Awake()
    {
        calculator = FindObjectOfType<VoxelGridCalculator>();
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

        onColliderDetectionFinished?.Invoke();

        // Debug.Log($"Voxel cost detection with optimization time taken: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
    }
}