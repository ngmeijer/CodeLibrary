using UnityEditor;
using UnityEngine;

public class VoxelCalculationConfirmPopup : EditorWindow
{
    public static bool HasClicked;
    public static bool HasContinued;
    public static bool HasCanceled;

    [MenuItem("Example/ShowPopup Example")]
    public static void Init()
    {
        HasClicked = false;
        HasContinued = false;
        VoxelCalculationConfirmPopup window = ScriptableObject.CreateInstance<VoxelCalculationConfirmPopup>();
        window.position = new Rect(1000, Screen.height / 2, 415, 170);
        window.ShowPopup();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField(
            "Are you sure you want to (re)calculate the voxel grid? This might take a while, depending on " +
            "\n\n-The voxel grid dimensions \n-Voxel size \n-The specifications of your computer.",
            EditorStyles.wordWrappedLabel);

        GUILayout.Space(10);
        using GUILayout.HorizontalScope horizontalScope = new GUILayout.HorizontalScope("box");

        {
            //Confirm button
            {
                GUI.backgroundColor = Color.green;
            }
            
            HasContinued = GUILayout.Button("Continue calculation",
                GUILayout.Width(200),
                GUILayout.Height(40));
            if (HasContinued)
            {
                HasClicked = true;
                HasContinued = true;
                Close();
            }

            //Cancel button
            {
                GUI.backgroundColor = Color.red;
            }
            
            HasCanceled = (GUILayout.Button("Cancel calculation",
                GUILayout.Width(200),
                GUILayout.Height(40)));
            if (HasCanceled)
            {
                HasClicked = true;
                Close();
            }
        }
    }
}