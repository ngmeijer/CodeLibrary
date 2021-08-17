using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneData : ScriptableObject
{
    public string SaveName;
    public string DateCreated;
    public SerializableDictionary<Vector3, BlockContainer> PlacedBlocks = new SerializableDictionary<Vector3, BlockContainer>();
}