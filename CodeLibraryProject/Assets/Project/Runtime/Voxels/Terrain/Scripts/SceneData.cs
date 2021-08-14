using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneData : ScriptableObject
{
    public SerializableDictionary<Vector3, BlockContainer> PlacedBlocks = new SerializableDictionary<Vector3, BlockContainer>();
}