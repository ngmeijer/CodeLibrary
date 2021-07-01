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

    [Header("Neighbour voxels")] [SerializeField]
    private bool showNeighbourVoxels;

    [SerializeField] private bool showNeighbourCosts;
    [SerializeField] private Color neighbourVoxelsColour = Color.yellow;

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
                Gizmos.DrawWireCube(currentVoxel.Position, voxelVisualSize);

                if (showCosts) visualizeVoxelCosts();
                if (showNeighbourVoxels) visualizeNeighbourVoxels();
            }
        }
    }

    private void visualizePath()
    {
        //Start voxel
        Gizmos.color = startVoxelColour;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(startVoxel.Position, $"Start voxel (= Voxel ID [ {startVoxel.ID} ]");
#endif
        Gizmos.DrawCube(startVoxel.Position, voxelVisualSize * 1.03f);

        //Target voxel
        Gizmos.color = targetVoxelColour;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(targetVoxel.Position, $"Target voxel (= Voxel ID [ {targetVoxel.ID} ]");
#endif

        Gizmos.DrawCube(targetVoxel.Position, voxelVisualSize);

        //Path
        Gizmos.color = pathVoxelColour;
        if (blackboard.path != null)
            for (int i = 0; i < blackboard.path.Count; i++)
            {
                Gizmos.DrawCube(blackboard.path[i].Position, voxelVisualSize);
            }
    }

    private void visualizeNeighbourVoxels()
    {
        if (currentVoxel == null)
            return;
        if (currentVoxel.neighbourData.neighbourVoxels == null)
            return;

        Dictionary<int, VoxelContainer> neighbourVoxels = currentVoxel.neighbourData.neighbourVoxels;

        foreach (KeyValuePair<int, VoxelContainer> voxel in neighbourVoxels)
        {
            Gizmos.color = neighbourVoxelsColour;
            Gizmos.DrawCube(voxel.Value.Position, voxelVisualSize);

#if UNITY_EDITOR
            if (showNeighbourCosts)
                UnityEditor.Handles.Label(voxel.Value.Position, $"ID: {voxel.Value.ID}" +
                                                                $"\nFcost: {voxel.Value.Fcost}" +
                                                                $"\nGcost: {voxel.Value.Gcost}" +
                                                                $"\nHcost: {voxel.Value.Hcost}");
            else UnityEditor.Handles.Label(voxel.Value.Position, $"ID: {voxel.Value.ID}");
#endif
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
        UnityEditor.Handles.Label(currentVoxel.Position - new Vector3(voxelVisualSize.x / 4, -voxelVisualSize.y / 4, 0),
            $"Gcost (distance this/start): {currentVoxel.Gcost}" +
            $"\nHcost (distance this/end): {currentVoxel.Hcost}" +
            $"\nFcost (total distance): {currentVoxel.Fcost}." +
            $"\n\nID: {currentVoxel.ID}" +
            $"\nStart pos: {startVoxel.Position}" +
            $"\nThis pos: {currentVoxel.Position}" +
            $"\nEnd pos: {targetVoxel.Position}", style);
#endif
    }
}