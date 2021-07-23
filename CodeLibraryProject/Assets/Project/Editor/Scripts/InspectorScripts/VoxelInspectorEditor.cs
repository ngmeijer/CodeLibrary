using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoxelGridCalculator))]
public class VoxelScriptEditor : UnityEditor.Editor
{
    private VoxelGridCalculator myTarget;
    private float inspectorWidth;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        myTarget = (VoxelGridCalculator)target;
        inspectorWidth = EditorGUIUtility.currentViewWidth;

        drawCalculateVoxelsGUI();
        drawClearVoxelsGUI();
    }

    private void drawCalculateVoxelsGUI()
    {
        GUI.backgroundColor = Color.green;
        GUIStyle recalculateBtnStyle = new GUIStyle(GUI.skin.button);
        recalculateBtnStyle.fontSize = 15;
        if (GUI.Button(new Rect(10, 35, inspectorWidth / 2 - 20, 50), "Recalculate voxels", recalculateBtnStyle))
        {
            GUI.backgroundColor = Color.green;
            myTarget.RecalculateVoxelGrid();
        }
    }

    private void drawClearVoxelsGUI()
    {
        GUI.backgroundColor = Color.red;
        GUIStyle clearBtnStyle = new GUIStyle(GUI.skin.button);
        clearBtnStyle.fontSize = 15;
        if (GUI.Button(new Rect(inspectorWidth / 2 + 10, 35, inspectorWidth / 2 - 20, 50), "Clear voxels", clearBtnStyle))
            myTarget.ClearVoxelData();
    }
}