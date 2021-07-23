using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct AStarDistanceJob : IJob
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
        int xDistance = (int) Mathf.Abs(pVoxel1.x - pVoxel2.x);
        int yDistance = (int) Mathf.Abs(pVoxel1.y - pVoxel2.y);
        int zDistance = (int) Mathf.Abs(pVoxel1.z - pVoxel2.z);

        return Mathf.Abs(xDistance - yDistance - zDistance);
    }
}