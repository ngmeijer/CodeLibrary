using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TerrainGenerator : MonoBehaviour
{
    [Space(70)] [SerializeField] private VoxelGridData saveFile;
    [SerializeField] private GameObject baseMeshPrefab;
    [SerializeField] private GameObject placedMeshPrefab;
    
    [SerializeField] private Transform pregeneratedBlockParent;
    [SerializeField] private Transform placedBlockParent;

    private List<GameObject> generatedMeshes = new List<GameObject>();
    private List<GameObject> placedMeshes = new List<GameObject>();
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
                GameObject instance = Instantiate(baseMeshPrefab, voxel.Value.WorldPosition, Quaternion.identity, pregeneratedBlockParent);
                voxel.Value.BlockInstance = instance;
                generatedMeshes.Add(instance);
                saveFile.ColliderVoxels.Add(voxel.Key, voxel.Value);
            }
        }

        meshCount = generatedMeshes.Count;
    }

    public void ClearTerrain()
    {
        saveFile.ColliderVoxels.Clear();
        
        destroyChildren(pregeneratedBlockParent.gameObject);
        destroyChildren(placedBlockParent.gameObject);

        generatedMeshes.Clear();
    }

    private void destroyChildren(GameObject pParent)
    {
        GameObject[] children = new GameObject[pParent.transform.childCount];

        for (int childIndex = 0; childIndex < pregeneratedBlockParent.childCount; childIndex++)
        {
            children[childIndex] = pParent.transform.GetChild(childIndex).gameObject;
        }

        foreach (GameObject child in children)
        {
            DestroyImmediate(child);
        }
    }

    public void HandleBlockAction(Vector3 pPosition, ActionType pType, RaycastHit pHit = new RaycastHit())
    {
        Vector3Int convertedPos = new Vector3Int((int) pPosition.x, (int) pPosition.y + 1, (int) pPosition.z);
     
        saveFile.VoxelPositions.TryGetValue(convertedPos, out int voxelID);
        saveFile.AllVoxels.TryGetValue(voxelID, out VoxelContainer voxel);

        if (voxel == null) return;

        switch (pType)
        {
            case ActionType.Place:
                voxel.IsTraversable = false;
                GameObject instance = Instantiate(placedMeshPrefab, voxel.WorldPosition, Quaternion.identity, placedBlockParent);
                placedMeshes.Add(instance);
                voxel.BlockInstance = instance;
                if (!saveFile.ColliderVoxels.ContainsKey(voxel.ID)) saveFile.ColliderVoxels.Add(voxel.ID, voxel);
                meshCount++;
                break;
            case ActionType.Remove:
                Destroy(pHit.collider.gameObject);
                voxel.IsTraversable = true;
                if(generatedMeshes.Contains(voxel.BlockInstance)) generatedMeshes.Remove(voxel.BlockInstance);
                if(placedMeshes.Contains(voxel.BlockInstance)) placedMeshes.Remove(voxel.BlockInstance);
                if (saveFile.ColliderVoxels.ContainsKey(voxel.ID)) saveFile.ColliderVoxels.Remove(voxel.ID);
                break;
        }
    }
}