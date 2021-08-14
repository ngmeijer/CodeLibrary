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
        drawClearButton();
        drawLoadFileButton();
        drawResetSceneButton();
    }

    private void drawGenerateButton()
    {
        GUI.backgroundColor = Color.green;
        GUIStyle generateBtnStyle = new GUIStyle(GUI.skin.button) {fontSize = 15};
        if (GUI.Button(new Rect(10, 35, inspectorWidth / 2 - 20, 40), "Generate terrain", generateBtnStyle))
        {
            editorTarget.GenerateTerrain();
        }
    }
    
    private void drawClearButton()
    {
        GUI.backgroundColor = Color.red;
        GUIStyle clearBtnStyle = new GUIStyle(GUI.skin.button) {fontSize = 15};
        if (GUI.Button(new Rect(inspectorWidth / 2 +10, 35, inspectorWidth / 2 - 20, 40), "Clear terrain", clearBtnStyle))
        {
            editorTarget.ClearTerrain();
        }
    }

    private void drawLoadFileButton()
    {
        GUI.backgroundColor = Color.yellow;
        GUIStyle saveSceneStyle = new GUIStyle(GUI.skin.button) {fontSize = 15};
        if (GUI.Button(new Rect(10, 85, inspectorWidth / 2 - 20, 40), "Load saved scene", saveSceneStyle))
        {
            editorTarget.LoadSavedScene();
        }
    }
    
    private void drawResetSceneButton()
    {
        GUI.backgroundColor = Color.grey;
        GUIStyle saveSceneStyle = new GUIStyle(GUI.skin.button) {fontSize = 15};
        if (GUI.Button(new Rect(inspectorWidth / 2 +10, 85, inspectorWidth / 2 - 20, 40), "Unload saved scene", saveSceneStyle))
        {
            editorTarget.UnloadScene();
        }
    }
}