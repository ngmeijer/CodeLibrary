using UnityEditor;
using UnityEngine;

public enum HandleDirection
{
    Right = 1,
    Up,
    Forward,
};

[CustomEditor(typeof(VoxelGridCalculator))]
public class VoxelGridEditor : Editor
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
    
    private Vector2 previousMousePos;
    private int nearestHandle;
    private float size = 3f;
    private Color currentColour;
    private HandleDirection direction;
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
        if (editorTarget == null) return;

        editorTargetTransform = editorTarget.transform;
        sceneDimensions = editorTarget.GetMapDimensions();

        Vector3 targetPosition = editorTarget.transform.position;
        Quaternion targetRotation = editorTargetTransform.rotation;

        Vector3 xAxisPosition = targetPosition + new Vector3(editorTarget.sceneDimensionsVector.x, editorTarget.sceneDimensionsVector.y / 2, editorTarget.sceneDimensionsVector.z / 2);
        Vector3 yAxisPosition = targetPosition + new Vector3(editorTarget.sceneDimensionsVector.x / 2, editorTarget.sceneDimensionsVector.y, editorTarget.sceneDimensionsVector.z / 2);
        Vector3 zAxisPosition = targetPosition + new Vector3(editorTarget.sceneDimensionsVector.x / 2, editorTarget.sceneDimensionsVector.y / 2, editorTarget.sceneDimensionsVector.z);

        if (Event.current.type == EventType.Repaint)
        {
            int hoverIndex = HandleUtility.nearestControl;

            {
                currentColour = hoverIndex == 1 ? focusColour : Handles.xAxisColor;
                Handles.color = currentColour;
                direction = HandleDirection.Right;
                Handles.ConeHandleCap(
                    (int) direction, xAxisPosition, targetRotation * Quaternion.LookRotation(Vector3.right), size, EventType.Repaint
                );

                currentColour = hoverIndex == 2 ? focusColour : Handles.yAxisColor;
                Handles.color = currentColour;
                direction = HandleDirection.Up;
                Handles.ConeHandleCap(
                    (int) direction, yAxisPosition, targetRotation * Quaternion.LookRotation(Vector3.up), size, EventType.Repaint
                );

                currentColour = hoverIndex == 3 ? focusColour : Handles.zAxisColor;
                Handles.color = currentColour;
                direction = HandleDirection.Forward;
                Handles.ConeHandleCap(
                    (int) direction, zAxisPosition, targetRotation * Quaternion.LookRotation(Vector3.forward), size, EventType.Repaint
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

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            nearestHandle = HandleUtility.nearestControl;
            previousMousePos = Event.current.mousePosition;
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            nearestHandle = -1;
            previousMousePos = Vector2.zero;
        }

        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
        {
            switch (nearestHandle)
            {
                case (int) HandleDirection.Right:
                    float moveX = HandleUtility.CalcLineTranslation(previousMousePos, Event.current.mousePosition,
                        editorTargetTransform.position, editorTargetTransform.right);
                    editorTarget.HandlesDelta += moveX * editorTargetTransform.right;
                    break;
                case (int) HandleDirection.Up:
                    float moveY = HandleUtility.CalcLineTranslation(previousMousePos, Event.current.mousePosition,
                        editorTargetTransform.position, editorTargetTransform.up);
                    editorTarget.HandlesDelta += moveY * editorTargetTransform.up;
                    break;
                case (int) HandleDirection.Forward:
                    float moveZ = HandleUtility.CalcLineTranslation(previousMousePos, Event.current.mousePosition,
                        editorTargetTransform.position, editorTargetTransform.forward);
                    editorTarget.HandlesDelta += moveZ * editorTargetTransform.forward;
                    break;
            }
        }

        previousMousePos = Event.current.mousePosition;

        pView.Repaint();
    }
}