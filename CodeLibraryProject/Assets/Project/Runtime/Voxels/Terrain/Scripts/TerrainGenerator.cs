using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class TerrainGenerator : MonoBehaviour
{
    [Space(70)] [SerializeField] private VoxelGridData saveFile;
    [SerializeField] private GameObject baseMeshPrefab;
    [SerializeField] private GameObject placedMeshPrefab;

    [SerializeField] private Transform pregeneratedBlockParent;
    [SerializeField] private Transform placedBlockParent;

    private float voxelSize;
    private Vector3 rayHitPosition;
    private Vector3 expectedPosition;
    private List<GameObject> generatedMeshes = new List<GameObject>();
    private List<GameObject> placedMeshes = new List<GameObject>();
    [SerializeField] [ReadOnlyInspector] private int meshCount;
    private VoxelContainer currentVoxel;

    public void GenerateTerrain()
    {
        ClearTerrain();

        voxelSize = saveFile.VoxelSize;
        baseMeshPrefab.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);

        foreach (KeyValuePair<int, VoxelContainer> voxel in saveFile.AllVoxels)
        {
            if (voxel.Value.GridPosition.y == 0)
            {
                GameObject instance = Instantiate(baseMeshPrefab, voxel.Value.WorldPosition, Quaternion.identity,
                    pregeneratedBlockParent);
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

    public void HandleBlockAction(ActionType pType, RaycastHit pHit)
    {
        Vector3 hitVoxelPos = pHit.collider.transform.position;
        Vector3 convertedPos = new Vector3(
             hitVoxelPos.x,
             hitVoxelPos.y,
             hitVoxelPos.z);

        rayHitPosition = pHit.point;
        voxelSize = saveFile.VoxelSize;
        
        expectedPosition = convertedPos;

        //Left/right hit
        if (rayHitPosition.x <= convertedPos.x - voxelSize / 2)
        {
            expectedPosition.x -= voxelSize;
        }
        if (rayHitPosition.x >= convertedPos.x + voxelSize / 2)
        {
            expectedPosition.x += voxelSize;
        }

        //Top/bottom hit
        if (rayHitPosition.y <= convertedPos.y - voxelSize / 2)
        {
            expectedPosition.y -= voxelSize;
        }
        if (rayHitPosition.y >= convertedPos.y + voxelSize / 2)
        {
            expectedPosition.y += voxelSize;
        }

        //Front/back hit
        if (rayHitPosition.z <= convertedPos.z - voxelSize / 2)
        {
            expectedPosition.z -= voxelSize;
        }
        if (rayHitPosition.z >= convertedPos.z + voxelSize / 2)
        {
            expectedPosition.z += voxelSize;
        }
        
        saveFile.VoxelPositions.TryGetValue(expectedPosition, out int voxelID);
        Debug.Log($"Expected pos: {expectedPosition}, ID: {voxelID}");
        saveFile.AllVoxels.TryGetValue(voxelID, out VoxelContainer voxel);
        currentVoxel = voxel;

        if (voxel == null) return;

        switch (pType)
        {
            case ActionType.Place:
                placeBlock(voxel);
                break;
            case ActionType.Remove:
                removeBlock(voxel, pHit);
                break;
        }
    }

    private void placeBlock(VoxelContainer pVoxel)
    {
        if (pVoxel.BlockInstance != null) return;

        pVoxel.IsTraversable = false;
        GameObject instance = Instantiate(placedMeshPrefab, pVoxel.WorldPosition, Quaternion.identity,
            placedBlockParent);
        placedMeshes.Add(instance);
        pVoxel.BlockInstance = instance;
        if (!saveFile.ColliderVoxels.ContainsKey(pVoxel.ID)) saveFile.ColliderVoxels.Add(pVoxel.ID, pVoxel);
        meshCount++;
        
        EditorUtility.SetDirty(saveFile);
    }

    private void removeBlock(VoxelContainer pVoxel, RaycastHit pHit)
    {
        Destroy(pHit.collider.gameObject);
        pVoxel.IsTraversable = true;
        if (generatedMeshes.Contains(pVoxel.BlockInstance)) generatedMeshes.Remove(pVoxel.BlockInstance);
        if (placedMeshes.Contains(pVoxel.BlockInstance)) placedMeshes.Remove(pVoxel.BlockInstance);
        if (saveFile.ColliderVoxels.ContainsKey(pVoxel.ID)) saveFile.ColliderVoxels.Remove(pVoxel.ID);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(expectedPosition, new Vector3(voxelSize * 0.9f, voxelSize * 0.9f, voxelSize * 0.9f));
        
        if (rayHitPosition == Vector3.zero) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(rayHitPosition, 0.1f);
    }
}