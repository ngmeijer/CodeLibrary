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
    }

    private void drawGenerateButton()
    {
        GUI.backgroundColor = Color.green;
        GUIStyle generateBtnStyle = new GUIStyle(GUI.skin.button) {fontSize = 15};
        if (GUI.Button(new Rect(10, 35, inspectorWidth / 2 - 20, 50), "Generate terrain", generateBtnStyle))
        {
            editorTarget.GenerateTerrain();
        }
    }
    
    private void drawClearButton()
    {
        GUI.backgroundColor = Color.red;
        GUIStyle clearBtnStyle = new GUIStyle(GUI.skin.button) {fontSize = 15};
        if (GUI.Button(new Rect(inspectorWidth / 2 +10, 35, inspectorWidth / 2 - 20, 50), "Clear terrain", clearBtnStyle))
        {
            editorTarget.ClearTerrain();
        }
    }
}
