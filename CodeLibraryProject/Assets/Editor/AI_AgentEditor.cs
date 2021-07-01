using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(AI_Agent), true)]
    public class AIAgentEditor : UnityEditor.Editor
    {
        private AI_Agent myTarget;
        private float inspectorWidth;
    
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            myTarget = (AI_Agent)target;
            inspectorWidth = EditorGUIUtility.currentViewWidth;

            drawCalculateAgentPathGUI();
        }

        private void drawCalculateAgentPathGUI()
        {
            GUI.backgroundColor = Color.green;
            GUIStyle recalculateBtnStyle = new GUIStyle(GUI.skin.button);
            recalculateBtnStyle.fontSize = 15;
            if (GUI.Button(new Rect(10, 35, inspectorWidth - 20, 50), "Request new Path", recalculateBtnStyle))
            {
                GUI.backgroundColor = Color.green;
                myTarget.RequestNewPath();
            }
        }
    }
}