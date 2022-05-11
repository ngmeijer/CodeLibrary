using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Dictionary<string, GameObject> saveInstanceHighlights = new Dictionary<string, GameObject>();
    private string currentlySelectedSave;
    private Dictionary<string, SceneData> foundSaves = new Dictionary<string, SceneData>();

    private void Awake()
    {
        if (_inst != null && _inst != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
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
        int saveUICount = savesParent.transform.childCount;
        for (int i = 0; i < saveUICount; i++)
        {
            Destroy(savesParent.transform.GetChild(i).gameObject);
        }
        foundSaves.Clear();
        saveGameCollection.Clear();
        saveInstanceHighlights.Clear();

        string[] assetNames = AssetDatabase.FindAssets("t:SceneData", new[] {"Assets/Resources/SceneData"});
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var save = AssetDatabase.LoadAssetAtPath<SceneData>(SOpath);
            foundSaves.Add(save.SaveName, save);
        }

        int index = 0;

        foreach (KeyValuePair<string, SceneData> save in foundSaves)
        {
            GameObject UIInstance = Instantiate(uiPrefab, savesParent.transform);
            SaveDataProcessor processor = UIInstance.GetComponent<SaveDataProcessor>();
            processor.ReceiveData(save.Value.SaveName, save.Value.DateCreated);
            saveGameCollection.Add(save.Value.SaveName, processor);

            GameObject highlightBackground = UIInstance.transform.GetChild(1).gameObject;
            highlightBackground.SetActive(false);
            saveInstanceHighlights.Add(save.Value.SaveName, highlightBackground);
        }
    }
    
    public void SaveToNewFile()
    {
        findSavedGames();
        terrainGenerator.SaveSceneToNewFile();
    }

    public void SaveToCurrentFile()
    {
        findSavedGames();
        terrainGenerator.SaveSceneToCurrentFile();
    }

    public void SelectSaveGame(string pName)
    {
        currentlySelectedSave = pName;

        foreach (KeyValuePair<string, GameObject> background in saveInstanceHighlights)
        {
            background.Value.SetActive(false);
        }

        saveInstanceHighlights.TryGetValue(pName, out GameObject currentBackground);
        currentBackground.SetActive(true);
    }

    public void LoadSaveGame()
    {
        foundSaves.TryGetValue(currentlySelectedSave, out SceneData data);

        terrainGenerator.LoadSavedScene(data);
    }
}