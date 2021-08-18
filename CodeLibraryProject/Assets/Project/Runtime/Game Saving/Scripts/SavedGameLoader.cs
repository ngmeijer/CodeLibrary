using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SavedGameLoader : MonoBehaviour
{
    //
    private static SavedGameLoader _inst;
    public static SavedGameLoader Inst => _inst;
    //
    
    private TerrainGenerator terrainGenerator;
    
    [SerializeField] private GameObject savesParent;
    [SerializeField] private GameObject uiPrefab;
    private Dictionary<string, SaveDataProcessor> saveGameCollection = new Dictionary<string, SaveDataProcessor>();
    private List<string> savesToDelete = new List<string>();
    private List<GameObject> saveInstanceHighlights = new List<GameObject>();
    private int currentlySelectedSave;
    
    private void Awake()
    {
        if (_inst != null && _inst != this)
        {
            Destroy(this.gameObject);
        } else {
            _inst = this;
        }
    }
    
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

        int index = 0;
        
        foreach (SceneData save in foundSaves)
        {
            GameObject UIInstance = Instantiate(uiPrefab, savesParent.transform);
            SaveDataProcessor processor = UIInstance.GetComponent<SaveDataProcessor>();
            processor.ReceiveData(save.SaveName, save.DateCreated);
            saveGameCollection.Add(save.SaveName, processor);
            processor.Index = index;
            index++;

            GameObject highlightBackground = UIInstance.transform.GetChild(1).gameObject;
            highlightBackground.SetActive(false);
            saveInstanceHighlights.Add(highlightBackground);
        }
    }

    public void AddToDeleteList(string pName)=> savesToDelete.Add(pName);

    public void LoadSavedScene() => terrainGenerator.LoadSavedScene();

    public void SaveGame() => terrainGenerator.SaveScene();

    public void DeleteSaveGames()
    {
        foreach (string saveName in savesToDelete)
        {
            string[] results = AssetDatabase.FindAssets(saveName);
        }
        
        savesToDelete.Clear();
    }

    public void SelectSaveGame(int pIndex)
    {
        currentlySelectedSave = pIndex;
        
        foreach(GameObject background in saveInstanceHighlights)
        {
            background.SetActive(false); 
        }
        
        saveInstanceHighlights[pIndex].SetActive(true);
    }
    
    public void HandleDelete(bool pState)
    {
        foreach (KeyValuePair<string, SaveDataProcessor> container in saveGameCollection)
        {
            Destroy(container.Value.gameObject);
        }
    }
}