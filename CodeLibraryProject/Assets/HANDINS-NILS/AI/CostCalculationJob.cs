using Shared_Scripts;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct CostCalculationJob : IJobParallelFor
{
    [ReadOnlyInspector] public float3 StartVoxelPos;
    [ReadOnlyInspector] public float3 TargetVoxelPos;
    [ReadOnlyInspector] public NativeArray<float3> AllVoxelPos;

    public NativeArray<int> GCostOutput;
    [ReadOnlyInspector] public NativeArray<int> HCostOutput;
    public NativeArray<int> FCostOutput;
    
    public void Execute(int pIndex)
    {
        GCostOutput[pIndex] = calculateCost(StartVoxelPos, AllVoxelPos[pIndex]);
        HCostOutput[pIndex] = calculateCost(TargetVoxelPos, AllVoxelPos[pIndex]);

        FCostOutput[pIndex] = GCostOutput[pIndex] + HCostOutput[pIndex];
    }
    
    private int calculateCost(float3 pVoxel1, float3 pVoxel2)
    {
        Vector3 distance = pVoxel1 - pVoxel2;
        float cost = (Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) + Mathf.Pow(distance.z, 2));
    
        return (int)cost * 10;
    }
}

[BurstCompile]
public struct NeighbourJob : IJob
{
    [ReadOnly] public float3 Voxel1Position;
    [ReadOnly] public float3 Voxel2Position;
    [ReadOnly] public int CurrentVoxelGCost;
    
    public NativeArray<int> NewGCostOutput;
    
    public void Execute()
    {
        NewGCostOutput[0] = CurrentVoxelGCost + calculateNeighbourDistance(Voxel1Position, Voxel2Position);
    }
    
    private int calculateNeighbourDistance(Vector3 pVoxel1, Vector3 pVoxel2)
    {
        int xDistance = (int)Mathf.Abs(pVoxel1.x - pVoxel2.x);
        int yDistance = (int)Mathf.Abs(pVoxel1.y - pVoxel2.y);
        int zDistance = (int)Mathf.Abs(pVoxel1.z - pVoxel2.z);

        return Mathf.Abs(xDistance - yDistance - zDistance);
    }
}