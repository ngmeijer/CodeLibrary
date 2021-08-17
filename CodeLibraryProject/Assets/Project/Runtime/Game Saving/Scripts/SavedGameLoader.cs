using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SavedGameLoader : MonoBehaviour
{
    [Space(130)] [SerializeField] private VoxelGridData voxelData;
    [SerializeField] private SceneData sceneData;

    private TerrainGenerator terrainGenerator;
    
    private void Start()
    {
        terrainGenerator = FindObjectOfType<TerrainGenerator>();
    }


    public void LoadSavedScene()
    {
        terrainGenerator.LoadSavedScene();
    }

    public void UnloadScene()
    {
        terrainGenerator.UnloadScene();
    }
}