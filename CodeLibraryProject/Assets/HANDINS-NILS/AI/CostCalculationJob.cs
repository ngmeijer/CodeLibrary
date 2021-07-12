using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct CostCalculationJob : IJobParallelFor
{
    public float3 StartVoxelPos;
    public float3 TargetVoxelPos;
    public NativeArray<float3> AllVoxelPos;

    public NativeArray<int> GCostOutput;
    public NativeArray<int> HCostOutput;
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

        return (int) cost * 10;
    }
}