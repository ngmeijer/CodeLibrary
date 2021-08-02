using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VoxelCalculationConfirmPopup : EditorWindow
{
    public static bool HasClicked;
    public static bool HasContinued;

    [MenuItem("Example/ShowPopup Example")]
    public static void Init()
    {
        HasClicked = false;
        HasContinued = false;
        VoxelCalculationConfirmPopup window = ScriptableObject.CreateInstance<VoxelCalculationConfirmPopup>();
        window.position = new Rect(1000, Screen.height / 2, 500, 175);
        window.ShowPopup();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField(
            "Are you sure you want to (re)calculate the voxel grid? This might take a while, depending on " +
            "\n-The voxel grid dimensions \n-Voxel size \n-The specifications of your computer.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Space(20);
        if (GUILayout.Button("Continue calculation"))
        {
            this.Close();
            HasClicked = true;
            HasContinued = true;
        }

        if (GUILayout.Button("Cancel calculation"))
        {
            this.Close();
            HasClicked = true;
        }
    }
}