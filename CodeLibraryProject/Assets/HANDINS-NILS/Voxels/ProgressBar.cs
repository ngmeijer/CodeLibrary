#if UNITY_EDITOR
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ProgressBar : EditorWindow
{
    private VoxelGridCalculator calculator;
    public static float MaxVoxelIndex;
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
}
#endif