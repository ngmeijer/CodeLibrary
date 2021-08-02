using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VoxelCalculationConfirmPopup : EditorWindow
{
    private bool hasConfirmed;

    [MenuItem("Example/ShowPopup Example")]
    public static void Init()
    {
        VoxelCalculationConfirmPopup window = ScriptableObject.CreateInstance<VoxelCalculationConfirmPopup>();
        window.position = new Rect(1000, Screen.height / 2, 500, 500);
        window.ShowPopup();
    }

    public static void RemoveAll()
    {
        VoxelCalculationConfirmPopup[] instances = FindObjectsOfType<VoxelCalculationConfirmPopup>();
        foreach (VoxelCalculationConfirmPopup instance in instances)
        {
            instance.ClosePopup();
        }
    }

    public void ClosePopup()
    {
        this.Close();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField(
            "Are you sure you want to (re)calculate the voxel grid? This might take a while, depending on " +
            "\n-The voxel grid dimensions \n-Voxel size \n-The specifications of your computer.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Space(70);
        if (GUILayout.Button("Continue calculation"))
        {
            this.Close();
            hasConfirmed = true;
        }

        if (GUILayout.Button("Cancel calculation"))
        {
            this.Close();
        }
    }
}