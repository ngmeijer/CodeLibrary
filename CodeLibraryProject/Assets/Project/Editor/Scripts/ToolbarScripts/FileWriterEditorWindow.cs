using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class FileWriterEditorWindow : EditorWindow
    {
        [MenuItem("" +
                  "Voxel asset" +
                  "/Create new voxel data container")]
        private static void ShowWindow()
        {
            var window = GetWindow<FileWriterEditorWindow>();
            window.titleContent = new GUIContent("Voxel saving");
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Save voxel data to asset"))
                createVoxelDataContainer();
        }

        private void createVoxelDataContainer()
        {
            VoxelGridData container = CreateInstance<VoxelGridData>();
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/SceneVoxelData/VoxelData.asset");
            AssetDatabase.CreateAsset(container, path);
        }
    }
}