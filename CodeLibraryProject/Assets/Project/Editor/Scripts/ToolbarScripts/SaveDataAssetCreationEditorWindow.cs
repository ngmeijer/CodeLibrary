using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class SaveDataAssetCreationEditorWindow : EditorWindow
    {
        [MenuItem("" +
                  "Save data asset" +
                  "/Create data container")]
        private static void ShowWindow()
        {
            var window = GetWindow<SaveDataAssetCreationEditorWindow>();
            window.titleContent = new GUIContent("Data saving");
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Save voxel data to asset"))
                createVoxelDataContainer();
            if (GUILayout.Button("Save scene data to asset"))
                createSceneDataContainer();
        }

        private void createVoxelDataContainer()
        {
            VoxelGridData container = CreateInstance<VoxelGridData>();
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/VoxelData/VoxelData.asset");
            AssetDatabase.CreateAsset(container, path);
        }
        
        private void createSceneDataContainer()
        {
            SceneData container = CreateInstance<SceneData>();
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/SceneData/SceneData.asset");
            AssetDatabase.CreateAsset(container, path);
        }
    }
}