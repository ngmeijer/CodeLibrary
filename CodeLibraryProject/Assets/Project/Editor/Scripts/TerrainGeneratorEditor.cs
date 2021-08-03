using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    private TerrainGenerator editorTarget;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        editorTarget = (TerrainGenerator) target;
    }
}
