using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    private TerrainGenerator editorTarget;
    private float inspectorWidth;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        editorTarget = (TerrainGenerator) target;
        inspectorWidth = EditorGUIUtility.currentViewWidth;

        drawGenerateButton();
        drawDeleteButton();
        drawLoadFileButton();
        drawUnloadSceneButton();
    }

    private void drawGenerateButton()
    {
        GUI.backgroundColor = Color.green;
        GUIStyle generateBtnStyle = new GUIStyle(GUI.skin.button) {fontSize = 15};
        if (GUI.Button(new Rect(inspectorWidth / 2, 35, inspectorWidth / 2 - 20, 25), "Generate terrain",
            generateBtnStyle))
        {
            editorTarget.GenerateTerrain();
        }
    }

    private void drawDeleteButton()
    {
        GUI.backgroundColor = Color.red;
        GUIStyle deleteBtnStyle = new GUIStyle(GUI.skin.button) {fontSize = 15};
        if (GUI.Button(new Rect(inspectorWidth / 2, 65, inspectorWidth / 2 - 20, 25), "Delete terrain", deleteBtnStyle))
        {
            editorTarget.DeleteTerrain();
        }
    }

    private void drawLoadFileButton()
    {
        GUI.backgroundColor = Color.yellow;
        GUIStyle saveSceneStyle = new GUIStyle(GUI.skin.button) {fontSize = 15};
        if (GUI.Button(new Rect(inspectorWidth / 2, 95, inspectorWidth / 2 - 20, 25), "Load saved scene",
            saveSceneStyle))
        {
            // editorTarget.LoadSavedScene();
        }
    }

    private void drawUnloadSceneButton()
    {
        GUI.backgroundColor = Color.grey;
        GUIStyle saveSceneStyle = new GUIStyle(GUI.skin.button) {fontSize = 15};
        if (GUI.Button(new Rect(inspectorWidth / 2, 125, inspectorWidth / 2 - 20, 25), "Unload saved scene",
            saveSceneStyle))
        {
            editorTarget.UnloadScene();
        }
    }
}