using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class TerrainGenerator : MonoBehaviour, IBlockInventoryHandler
{
    [Space(110)] [SerializeField] private VoxelGridData voxelData;
    [SerializeField] private SceneData sceneData;
    [SerializeField] private GameObject baseMeshPrefab;
    [SerializeField] private Transform pregeneratedBlockParent;
    [SerializeField] private Transform placedBlockParent;

    private GameObject currentSelectedBlockPrefab;
    private float voxelSize;
    private Vector3 rayHitPosition;
    private Vector3 expectedPosition;
    private List<GameObject> generatedMeshes = new List<GameObject>();
    private List<GameObject> placedMeshes = new List<GameObject>();
    [SerializeField] [ReadOnlyInspector] private int meshCount;
    private int index;
    private string currentBlockName;
    public List<string> blockNames;

    private SerializableDictionary<string, GameObject> blockCollection =
        new SerializableDictionary<string, GameObject>();

    private void Start()
    {
        blockNames = GameManager.Instance.GetBlockNames();
        fillBlockDict();
    }

    private void Update()
    {
        CycleThroughBlocks();
    }

    public void LoadSavedScene()
    {
        GenerateTerrain();
        foreach (BlockContainer block in sceneData.PlacedBlocks.Values)
        {
            GameObject prefab = GetProperBlockPrefab(block.BlockType);
            Instantiate(prefab, block.WorldPosition, Quaternion.identity, placedBlockParent);
            block.voxel.IsTraversable = false;

            voxelData.VoxelPositions.TryGetValue(block.WorldPosition, out int voxelID);
            voxelData.AllVoxels.TryGetValue(voxelID, out VoxelContainer voxel);
            voxelData.ColliderVoxels.Add(voxelID, voxel);
        }

        EditorUtility.SetDirty(voxelData);
    }

    public void UnloadScene()
    {
        ClearPlacedBlocks();
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        ClearTerrain();

        voxelSize = voxelData.VoxelSize;
        baseMeshPrefab.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);

        foreach (KeyValuePair<int, VoxelContainer> voxel in voxelData.AllVoxels)
        {
            if (voxel.Value.GridPosition.y == 0)
            {
                GameObject instance = Instantiate(baseMeshPrefab, voxel.Value.WorldPosition, Quaternion.identity,
                    pregeneratedBlockParent);
                voxel.Value.BlockInstance = instance;
                generatedMeshes.Add(instance);
                voxelData.ColliderVoxels.Add(voxel.Key, voxel.Value);
            }
        }

        meshCount = generatedMeshes.Count;
    }

    private void fillBlockDict()
    {
        string path = "Prefabs/Blocks/Prefab_";

        blockCollection.Clear();

        LoadResourceFromAssets("Grass");
        LoadResourceFromAssets("Stone");
        LoadResourceFromAssets("DarkWood");
        LoadResourceFromAssets("Dirt");
        LoadResourceFromAssets("LightWood");
    }

    public void LoadResourceFromAssets(string pType)
    {
        const string path = "Prefabs/Blocks/Prefab_";

        GameObject blockPrefab = Resources.Load(path + pType) as GameObject;
        blockCollection.Add(pType, blockPrefab);
    }

    public void ClearTerrain()
    {
        voxelData.ColliderVoxels.Clear();

        destroyChildren(pregeneratedBlockParent);
        destroyChildren(placedBlockParent);

        generatedMeshes.Clear();
    }

    public void ClearPlacedBlocks()
    {
        foreach (GameObject block in placedMeshes)
        {
            voxelData.VoxelPositions.TryGetValue(block.transform.position, out int voxelID);
            voxelData.ColliderVoxels.Remove(voxelID);
        }
        destroyChildren(placedBlockParent);
        placedMeshes.Clear();
    }

    private void destroyChildren(Transform pParent)
    {
        GameObject[] children = new GameObject[pParent.childCount];

        for (int childIndex = 0; childIndex < pParent.childCount; childIndex++)
        {
            children[childIndex] = pParent.GetChild(childIndex).gameObject;
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
        voxelSize = voxelData.VoxelSize;

        expectedPosition = convertedPos;

        //Left/right hit
        if (rayHitPosition.x <= convertedPos.x - voxelSize / 2)
            expectedPosition.x -= voxelSize;
        if (rayHitPosition.x >= convertedPos.x + voxelSize / 2)
            expectedPosition.x += voxelSize;

        //Top/bottom hit
        if (rayHitPosition.y <= convertedPos.y - voxelSize / 2)
            expectedPosition.y -= voxelSize;
        if (rayHitPosition.y >= convertedPos.y + voxelSize / 2)
            expectedPosition.y += voxelSize;

        //Front/back hit
        if (rayHitPosition.z <= convertedPos.z - voxelSize / 2)
            expectedPosition.z -= voxelSize;
        if (rayHitPosition.z >= convertedPos.z + voxelSize / 2)
            expectedPosition.z += voxelSize;

        voxelData.VoxelPositions.TryGetValue(expectedPosition, out int voxelID);
        voxelData.AllVoxels.TryGetValue(voxelID, out VoxelContainer voxel);

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
        //Check if block is already in voxel
        if (pVoxel.BlockInstance != null) return;

        //Create block GameObject
        GameObject instance = Instantiate(currentSelectedBlockPrefab, pVoxel.WorldPosition, Quaternion.identity,
            placedBlockParent);
        placedMeshes.Add(instance);

        //Handle voxel modifications
        pVoxel.IsTraversable = false;
        pVoxel.BlockInstance = instance;
        if (!voxelData.ColliderVoxels.ContainsKey(pVoxel.ID)) voxelData.ColliderVoxels.Add(pVoxel.ID, pVoxel);

        //Create block data for scene saving & loading
        BlockContainer block = new BlockContainer()
        {
            BlockType = currentBlockName,
            WorldPosition = pVoxel.WorldPosition,
            voxel = pVoxel
        };
        if (!sceneData.PlacedBlocks.ContainsKey(block.WorldPosition))
            sceneData.PlacedBlocks.Add(block.WorldPosition, block);
        EditorUtility.SetDirty(voxelData);
        EditorUtility.SetDirty(sceneData);

        meshCount++;
    }

    private void removeBlock(VoxelContainer pVoxel, RaycastHit pHit)
    {
        Destroy(pHit.collider.gameObject);
        pVoxel.IsTraversable = true;
        
        if (generatedMeshes.Contains(pVoxel.BlockInstance)) generatedMeshes.Remove(pVoxel.BlockInstance);
        if (placedMeshes.Contains(pVoxel.BlockInstance)) placedMeshes.Remove(pVoxel.BlockInstance);
        if (voxelData.ColliderVoxels.ContainsKey(pVoxel.ID)) voxelData.ColliderVoxels.Remove(pVoxel.ID);
        if (sceneData.PlacedBlocks.ContainsKey(pVoxel.WorldPosition)) sceneData.PlacedBlocks.Remove(pVoxel.WorldPosition);
    }

    public void CycleThroughBlocks()
    {
        if (!Application.isPlaying) return;

        float mouseWheel = InputManager.Instance.MouseWheel;
        int highestIndex = blockNames.Count - 1;

        if (mouseWheel < 0)
        {
            if (index <= 0)
                index = highestIndex;
            else index--;
        }

        if (mouseWheel > 0)
        {
            if (index >= highestIndex)
                index = 0;
            else index++;
        }

        currentBlockName = blockNames[index];

        blockCollection.TryGetValue(currentBlockName, out currentSelectedBlockPrefab);
    }

    public GameObject GetProperBlockPrefab(string pType)
    {
        blockCollection.TryGetValue(pType, out GameObject block);
        return block;
    }
}