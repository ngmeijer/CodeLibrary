using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoxelGridEditor))]
public class VoxelGridEditor : Editor
{
    private VoxelGridCalculator editorTarget;
    private Vector3 xAxisGrid;
    
    public void OnSceneGUI()
    {
        editorTarget = target as VoxelGridCalculator;

        xAxisGrid = editorTarget.CalculatorPosition + new Vector3(editorTarget.sceneDimensionsVector.x, 0, 0);
        Handles.color = Handles.xAxisColor;
        Handles.ArrowHandleCap(
            0, 
            xAxisGrid + new Vector3(30f, 0f, 0f),
            Quaternion.identity * Quaternion.LookRotation(Vector3.right),
            1f,
            EventType.Repaint
        );

    }
}