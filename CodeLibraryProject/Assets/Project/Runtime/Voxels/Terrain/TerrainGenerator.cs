using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    [Space(70)]
    [SerializeField] private VoxelGridData saveFile;
    [SerializeField] private GameObject meshPrefab;

    private List<GameObject> generatedMeshes = new List<GameObject>();
    
    public void GenerateTerrain()
    {
        foreach (KeyValuePair<int, VoxelContainer> voxel in saveFile.AllVoxels)
        {
            if (voxel.Value.GridPosition.y == 0)
            {
                GameObject instance = Instantiate(meshPrefab, voxel.Value.WorldPosition, Quaternion.identity);
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
}
