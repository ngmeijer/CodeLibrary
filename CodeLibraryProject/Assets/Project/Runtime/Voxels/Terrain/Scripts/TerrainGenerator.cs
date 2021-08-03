using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TerrainGenerator : MonoBehaviour
{
    [Space(70)] [SerializeField] private VoxelGridData saveFile;
    [SerializeField] private GameObject baseMeshPrefab;
    [SerializeField] private GameObject placedMeshPrefab;
    
    [SerializeField] private Transform parent;

    private List<GameObject> generatedMeshes = new List<GameObject>();
    [SerializeField] [ReadOnlyInspector] private int meshCount;
    
    public void GenerateTerrain()
    {
        ClearTerrain();

        float voxelSize = saveFile.VoxelSize;
        baseMeshPrefab.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);

        foreach (KeyValuePair<int, VoxelContainer> voxel in saveFile.AllVoxels)
        {
            if (voxel.Value.GridPosition.y == 0)
            {
                GameObject instance = Instantiate(baseMeshPrefab, voxel.Value.WorldPosition, Quaternion.identity, parent);
                generatedMeshes.Add(instance);
                saveFile.ColliderVoxels.Add(voxel.Key, voxel.Value);
            }
        }

        meshCount = generatedMeshes.Count;
    }

    public void ClearTerrain()
    {
        saveFile.ColliderVoxels.Clear();

        GameObject[] children = new GameObject[parent.childCount];
        
        for (int childIndex = 0; childIndex < parent.childCount; childIndex++)
        {
            children[childIndex] = parent.GetChild(childIndex).gameObject;
        }

        foreach (GameObject child in children)
        {
            DestroyImmediate(child);
        }

        generatedMeshes.Clear();
    }

    public void ReceiveSelectedVoxelPosition(Vector3 pPosition, ActionType pType)
    {
        Vector3Int convertedPos = new Vector3Int((int) pPosition.x, (int) pPosition.y, (int) pPosition.z);
     
        saveFile.VoxelPositions.TryGetValue(convertedPos, out int voxelID);
        saveFile.AllVoxels.TryGetValue(voxelID, out VoxelContainer voxel);

        if (voxel == null) return;

        voxel.IsTraversable = false;
        GameObject instance = Instantiate(placedMeshPrefab, voxel.WorldPosition, Quaternion.identity, parent);
        generatedMeshes.Add(instance);

        meshCount++;

        if (!saveFile.ColliderVoxels.ContainsKey(voxel.ID)) saveFile.ColliderVoxels.Add(voxel.ID, voxel);
    }
}