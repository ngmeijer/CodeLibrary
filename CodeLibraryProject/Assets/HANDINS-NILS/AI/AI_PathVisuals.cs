using System.Collections.Generic;
using UnityEngine;

public class AI_PathVisuals : MonoBehaviour
{
    private Dictionary<int, VoxelContainer> allTraversableVoxels;

    [SerializeField] private bool showVisualization = false;

    [Header("Focused debug voxel")] [SerializeField]
    private bool showFocusedVoxel;

    [SerializeField] private bool showCosts;
    [SerializeField] private int currentVoxelID;
    [SerializeField] private Color focusedVoxelColour = Color.white;
    
    [Header("A* path")] [SerializeField] private bool showPath;
    [SerializeField] private Color startVoxelColour = Color.green;
    [SerializeField] private Color targetVoxelColour = Color.red;
    [SerializeField] private Color pathVoxelColour = Color.black;

    private Vector3 voxelVisualSize;
    private VoxelContainer currentVoxel;
    private VoxelContainer startVoxel;
    private VoxelContainer targetVoxel;
    [SerializeField] private AI_Blackboard blackboard;

    private void Start()
    {
        getBlackboardReference();
        UpdatePathfindingData();
        showVisualization = true;
    }

    public void UpdatePathfindingData()
    {
        if (blackboard == null)
        {
            Debug.LogError("AI_Blackboard reference is null.");
            return;
        }

        allTraversableVoxels = blackboard.GetAllTraversableVoxels();
    }

    private void getBlackboardReference()
    {
        UpdatePathfindingData();
    }

    private void OnDrawGizmos()
    {
        if (showVisualization)
        {
            UpdatePathfindingData();
            if (allTraversableVoxels == null) return;
            if (allTraversableVoxels.Count == 0) return;

            float voxelSize = blackboard.GetVoxelSize();
            voxelVisualSize = new Vector3(voxelSize, voxelSize, voxelSize);

            targetVoxel = blackboard.targetVoxel;
            startVoxel = blackboard.startVoxel;

            if (showPath) visualizePath();

            if (showFocusedVoxel)
            {
                if (!allTraversableVoxels.ContainsKey(currentVoxelID))
                    return;
                currentVoxel = allTraversableVoxels[currentVoxelID];
                Gizmos.color = focusedVoxelColour;
                Gizmos.DrawWireCube(currentVoxel.WorldPosition, voxelVisualSize);

                if (showCosts) visualizeVoxelCosts();
            }
        }
    }

    private void visualizePath()
    {
        //Start voxel
        Gizmos.color = startVoxelColour;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(startVoxel.WorldPosition, $"Start voxel (= Voxel ID [ {startVoxel.ID} ]");
#endif
        Gizmos.DrawCube(startVoxel.WorldPosition, voxelVisualSize * 1.03f);

        //Target voxel
        Gizmos.color = targetVoxelColour;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(targetVoxel.WorldPosition, $"Target voxel (= Voxel ID [ {targetVoxel.ID} ]");
#endif

        Gizmos.DrawCube(targetVoxel.WorldPosition, voxelVisualSize);

        //Path
        Gizmos.color = pathVoxelColour;
        if (blackboard.path != null)
            for (int i = 0; i < blackboard.path.Count; i++)
            {
                Gizmos.DrawCube(blackboard.path[i].WorldPosition, voxelVisualSize);
            }
    }

    private void visualizeVoxelCosts()
    {
        if (currentVoxelID > allTraversableVoxels.Count)
            return;

        currentVoxelID = (int) Mathf.Clamp(currentVoxelID, 0, allTraversableVoxels.Count);
        Gizmos.color = Color.white;
        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        Gizmos.color = Color.white;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(currentVoxel.WorldPosition - new Vector3(voxelVisualSize.x / 4, -voxelVisualSize.y / 4, 0),
            $"Gcost (distance this/start): {currentVoxel.Gcost}" +
            $"\nHcost (distance this/end): {currentVoxel.Hcost}" +
            $"\nFcost (total distance): {currentVoxel.Fcost}." +
            $"\n\nID: {currentVoxel.ID}" +
            $"\nStart pos: {startVoxel.WorldPosition}" +
            $"\nThis pos: {currentVoxel.WorldPosition}" +
            $"\nEnd pos: {targetVoxel.WorldPosition}", style);
#endif
    }
}