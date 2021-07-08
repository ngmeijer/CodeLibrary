#if UNITY_EDITOR
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ProgressBar : EditorWindow
{
    private VoxelGridCalculator calculator;
    public static int MaxVoxelIndex;
    public static bool HasFinishedProcess;

    public static void ShowVoxelCreateProgress(int pCurrentVoxelIndex)
    {
        EditorUtility.DisplayProgressBar("Voxel creation progress",
            $"Creating voxels... {pCurrentVoxelIndex}/{MaxVoxelIndex}",
            pCurrentVoxelIndex / MaxVoxelIndex);

        if (pCurrentVoxelIndex < MaxVoxelIndex) return;
        if (!HasFinishedProcess) return;

        EditorUtility.ClearProgressBar();
        HasFinishedProcess = false;
    }

    public static void ShowVoxelCollisionProgress(int pCurrentVoxelIndex)
    {
        EditorUtility.DisplayProgressBar("Voxel collision detection progress",
            $"Detecting if voxels are traversable... {pCurrentVoxelIndex}/{MaxVoxelIndex}",
            pCurrentVoxelIndex / MaxVoxelIndex);

        if (pCurrentVoxelIndex < MaxVoxelIndex) return;
        if (!HasFinishedProcess) return;

        EditorUtility.ClearProgressBar();
        HasFinishedProcess = false;
    }
    
    public static void ShowVoxelNeighbourProgress(int pCurrentVoxelIndex)
    {
        EditorUtility.DisplayProgressBar("Voxel neighbour detection progress",
            $"Detecting what voxels are neighbours of current voxel... {pCurrentVoxelIndex}/{MaxVoxelIndex}",
            pCurrentVoxelIndex / MaxVoxelIndex);

        if (pCurrentVoxelIndex < MaxVoxelIndex) return;
        if (!HasFinishedProcess) return;

        EditorUtility.ClearProgressBar();
        HasFinishedProcess = false;
    }
}
#endif