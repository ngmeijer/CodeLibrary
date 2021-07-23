using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoxelGridCalculator)), CanEditMultipleObjects]
public class SceneArrowHandleGUI : Editor
{
    private void OnSceneGUI()
    {
        VoxelGridCalculator editorTarget = (VoxelGridCalculator)target;

        float size = HandleUtility.GetHandleSize(editorTarget.CalculatorPosition + new Vector3(editorTarget.sceneDimensionsVector[0],0 ,0)) * 1.5f;
        float snap = 0.1f;

        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.Slider(editorTarget.CalculatorPosition, Vector3.right, size, Handles.DotHandleCap, snap);
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(editorTarget, "Change Look At Target Position");
            editorTarget.UpdateGridSizeThroughHandles();
        }
    }
}