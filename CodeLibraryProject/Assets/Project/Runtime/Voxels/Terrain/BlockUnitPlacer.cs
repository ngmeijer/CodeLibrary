using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUnitPlacer : MonoBehaviour
{
    [SerializeField] private VoxelGridData saveFile;

    public void ReceiveSelectedVoxelPosition(Vector3 pPosition)
    {
        Vector3Int convertedPos = new Vector3Int((int)pPosition.x, (int)pPosition.y, (int)pPosition.z);
        saveFile.VoxelPositions.TryGetValue(convertedPos, out int voxelID);

        saveFile.AllVoxels.TryGetValue(voxelID, out VoxelContainer voxel);
        
        Debug.Log($"Placing block at {pPosition} with voxelID {voxel.ID}");
    }
}
