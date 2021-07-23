using System.Collections.Generic;
using UnityEngine;

public static class VoxelPositionHandler
{
    public static VoxelContainer GetVoxelFromWorldPos(Dictionary<int, VoxelContainer> pAllVoxels, Vector3 pPosition, Vector3 pCalculatorPosition,float[] pDimensions, float pVoxelSize)
    {
        VoxelContainer container = new VoxelContainer();

        float xPercent = (pPosition.x) / pDimensions[0];
        float yPercent = (pPosition.y) / pDimensions[1];
        float zPercent = (pPosition.z) / pDimensions[2];

        float xPos = xPercent * pDimensions[0];
        float yPos = yPercent * pDimensions[1];
        float zPos = zPercent * pDimensions[2];

        Vector3 gridPosition = new Vector3(xPos, yPos, zPos) - pCalculatorPosition;
        
        foreach (KeyValuePair<int, VoxelContainer> voxel in pAllVoxels)
        {
            if (!checkIfPositionInsideVoxel(gridPosition, voxel.Value.WorldPosition, pVoxelSize)) continue;
            
            container = voxel.Value;
            break;
        }

        return container;
    }

    private static bool checkIfPositionInsideVoxel(Vector3 pGridPosition, Vector3 pVoxelPosition, float pVoxelVisualSize)
    {
        bool xInRange = pGridPosition.x >= pVoxelPosition.x - pVoxelVisualSize / 2 && pGridPosition.x <= pVoxelPosition.x + pVoxelVisualSize / 2;
        if (!xInRange) return false;
        bool yInRange = pGridPosition.y >= pVoxelPosition.y - pVoxelVisualSize / 2 && pGridPosition.y <= pVoxelPosition.y + pVoxelVisualSize / 2;
        if (!yInRange) return false;
        bool zInRange = pGridPosition.z >= pVoxelPosition.z - pVoxelVisualSize / 2 && pGridPosition.z <= pVoxelPosition.z + pVoxelVisualSize / 2;
        if (!zInRange) return false;
        
        return true;
    }
}