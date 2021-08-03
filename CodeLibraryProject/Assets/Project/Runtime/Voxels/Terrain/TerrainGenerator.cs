using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TerrainGenerator : MonoBehaviour
{
    [Space(70)] [SerializeField] private VoxelGridData saveFile;
    [SerializeField] private GameObject meshPrefab;
    [SerializeField] private Transform parent;
    
    [SerializeField] private List<GameObject> generatedMeshes = new List<GameObject>();

    public void GenerateTerrain()
    {
        ClearTerrain();

        foreach (KeyValuePair<int, VoxelContainer> voxel in saveFile.AllVoxels)
        {
            if (voxel.Value.GridPosition.y == 0)
            {
                GameObject instance = Instantiate(meshPrefab, voxel.Value.WorldPosition, Quaternion.identity, parent);
                generatedMeshes.Add(instance);
            }
        }
    }

    public void ClearTerrain()
    {
        foreach (GameObject instance in generatedMeshes)
        {
            DestroyImmediate(instance);
        }

        generatedMeshes.Clear();
    }

    public void ReceiveSelectedVoxelPosition(Vector3 pPosition)
    {
        Vector3Int convertedPos = new Vector3Int((int) pPosition.x, (int) pPosition.y + 1, (int) pPosition.z);
        saveFile.VoxelPositions.TryGetValue(convertedPos, out int voxelID);

        saveFile.AllVoxels.TryGetValue(voxelID, out VoxelContainer voxel);

        if (voxel == null) return;

        voxel.IsTraversable = false;
        GameObject instance = Instantiate(meshPrefab, voxel.WorldPosition, Quaternion.identity, parent);
        generatedMeshes.Add(instance);
    }
}