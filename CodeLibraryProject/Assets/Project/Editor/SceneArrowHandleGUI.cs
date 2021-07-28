using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoxelGridCalculator))]
public class DotExampleEditor : Editor
{
    private float size = 3f;
    private Color currentColour;
    private Color focusColour = Color.white;
    private float[] sceneDimensions;
    private VoxelGridCalculator editorTarget;
    private Transform editorTargetTransform;

    private void OnEnable()
    {
        SceneView.duringSceneGui += CustomOnSceneGUI;
    }

    private void CustomOnSceneGUI(SceneView pView)
    {
        editorTarget = (VoxelGridCalculator) target;
        editorTargetTransform = editorTarget.transform;
        sceneDimensions = editorTarget.GetMapDimensions();
        int hoverIndex = -1;

        Vector3 targetPosition = editorTarget.transform.position;
        Quaternion targetRotation = editorTargetTransform.rotation;
        
        Vector3 xAxisPosition = targetPosition + new Vector3(50, sceneDimensions[1] / 2, sceneDimensions[2] / 2);
        Vector3 yAxisPosition = targetPosition + new Vector3(sceneDimensions[0] / 2, 50, sceneDimensions[2] / 2);
        Vector3 zAxisPosition = targetPosition + new Vector3(sceneDimensions[0] / 2, sceneDimensions[1] / 2, 50);

        if (Event.current.type == EventType.Repaint)
        {
            hoverIndex = HandleUtility.nearestControl;

            {
                currentColour = hoverIndex == 1 ? focusColour : Handles.xAxisColor;
                Handles.color = currentColour;
                Handles.ConeHandleCap(
                    1, xAxisPosition, targetRotation * Quaternion.LookRotation(Vector3.right), size,
                    EventType.Repaint
                );

                currentColour = hoverIndex == 2 ? focusColour : Handles.yAxisColor;
                Handles.color = currentColour;
                Handles.ConeHandleCap(
                    2, yAxisPosition, targetRotation * Quaternion.LookRotation(Vector3.up), size,
                    EventType.Repaint
                );

                currentColour = hoverIndex == 3 ? focusColour : Handles.zAxisColor;
                Handles.color = currentColour;
                Handles.ConeHandleCap(
                    3, zAxisPosition, targetRotation * Quaternion.LookRotation(Vector3.forward), size,
                    EventType.Repaint
                );
            }
        }

        if (Event.current.type == EventType.Layout)
        {
            Handles.ConeHandleCap(
                1, xAxisPosition, targetRotation * Quaternion.LookRotation(Vector3.right), size,
                EventType.Layout
            );
            
            Handles.ConeHandleCap(
                2, yAxisPosition, targetRotation * Quaternion.LookRotation(Vector3.up), size,
                EventType.Layout
            );
            
            Handles.ConeHandleCap(
                3, zAxisPosition, targetRotation * Quaternion.LookRotation(Vector3.forward), size,
                EventType.Layout
            );
        }
        
        pView.Repaint();
    }
}