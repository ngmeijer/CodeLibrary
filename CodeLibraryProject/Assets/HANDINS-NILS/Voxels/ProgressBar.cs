#if UNITY_EDITOR
using System.Threading;
using UnityEditor;
using UnityEngine;

// Shows a progress bar for the specified number of seconds.
public class VoxelGridProgressBar : EditorWindow
{
    private VoxelGridCalculator calculator;
    public static float MaxVoxelCount;

    public static void ShowProgressBar(int pCurrentVoxelIndex)
    {
        while (pCurrentVoxelIndex < MaxVoxelCount)
        {
            EditorUtility.DisplayProgressBar("Voxel creation progress",
                $"Creating voxels... {pCurrentVoxelIndex}/{MaxVoxelCount}",
                pCurrentVoxelIndex / MaxVoxelCount);
            return;
        }

        EditorUtility.ClearProgressBar();
    }
}
#endif