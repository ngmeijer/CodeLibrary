using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class SavedGameLoader : MonoBehaviour
{
    private TerrainGenerator terrainGenerator;
    
    [SerializeField] private GameObject savesParent;
    [SerializeField] private GameObject uiPrefab;

    private void Start()
    {
        terrainGenerator = FindObjectOfType<TerrainGenerator>();
        
        findSavedGames();
    }
    
    private void findSavedGames()
    {
        // search for a ScriptObject called ScriptObj
        string[] assetNames = AssetDatabase.FindAssets("t:SceneData", new[] { "Assets/Resources/SceneData" });
        List<SceneData> foundSaves = new List<SceneData>();
        foreach (string SOName in assetNames)
        {
            var SOpath= AssetDatabase.GUIDToAssetPath(SOName);
            var save = AssetDatabase.LoadAssetAtPath<SceneData>(SOpath);
            foundSaves.Add(save);
        }
        
        Debug.Log(foundSaves.Count);

        foreach (SceneData save in foundSaves)
        {
            GameObject UIInstance = Instantiate(uiPrefab, savesParent.transform);
            SaveDataProcessor processor = UIInstance.GetComponent<SaveDataProcessor>();
            processor.ReceiveData(save.SaveName, save.DateCreated);
        }
    }

    public void LoadSavedScene() => terrainGenerator.LoadSavedScene();

    public void SaveGame() => terrainGenerator.SaveScene();
}