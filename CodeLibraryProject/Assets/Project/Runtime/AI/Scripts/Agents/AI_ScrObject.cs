using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI Type", menuName = "ScriptableObjects/AI", order = 1)]
public class AI_ScrObject : ScriptableObject
{
    [Header("Utility variables")]
    [Range(1f, 100f)]public float MaxSight;
    public float MinRandomValue;
    public float MaxRandomValue; 
    [Range(0.01f, 5f)]public float UtilityInterval = 0.5f;
    [Range(0.01f, 5f)] public float PathCalculationInterval = 0.1f;

    public SerializableDictionary<int, string> raycastTags;
    
    [Header("Gameplay variables")]
    [Range(0.01f, 2f)] public float StoppingDistance = 0.5f;
    [Range(1f, 50f)] public float NormalMoveSpeed = 5f;
    [Range(1f, 50f)] public float ChaseSpeed = 9f;
    [Range(0.5f, 25f)] public float MinWalkRange = 2.5f;
    [Range(5f, 50f)] public float MaxWalkRange = 10f;
    [Range(0.1f, 30f)] public float AttackRange = 2f;
    [Range(0.01f, 5f)] public float AttackSpeed = 0.5f;
    
    [Header("Gizmos customization")]
    public Color SightVisuals = Color.yellow;

    [Header("Player")] 
    public float Distance_PlayerFactor;
    public float Type_PlayerFactor;
    
    [Header("Bee")]
    [Tooltip("")] public float Distance_BeeFactor;
    [Tooltip("Higher value means a higher priority")] public float Type_BeeFactor;

    [Header("Skunk")]
    [Tooltip("Higher value means a higher priority")] public float Type_SkunkFactor;
    [Tooltip("Higher value means a higher priority")] public float Distance_SkunkFactor;
}