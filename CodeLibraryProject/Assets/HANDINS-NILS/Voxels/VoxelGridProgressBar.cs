#if UNITY_EDITOR
using System.Threading;
using UnityEditor;
using UnityEngine;

// Shows a progress bar for the specified number of seconds.
public class VoxelGridProgressBar : EditorWindow
{
    private VoxelGridCalculator calculator;
    public static float secs = 15f;

    public static void ShowProgressBar()
    {
        var step = 0.1f;
        for (float t = 0; t < secs; t += step)
        {
            EditorUtility.DisplayProgressBar("Simple Progress Bar", "Doing some work...", t / secs);
            // Normally, some computation happens here.
            // This example uses Sleep.
            Thread.Sleep((int) (step * 50.0f));
        }

        EditorUtility.ClearProgressBar();
    }
}
#endif