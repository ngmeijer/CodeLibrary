using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class AI_PathFinder : MonoBehaviour
{
    private Dictionary<int, VoxelContainer> traversableVoxels = new Dictionary<int, VoxelContainer>();
    private Dictionary<Vector3Int, int> voxelGridPositions = new Dictionary<Vector3Int, int>();
    private float[] mapDimensions;
    private float voxelSize;
    private Vector3 thisPosition;
    private AI_Blackboard blackboard;
    private AI_PathVisuals pathVisualizer;
    
    [ReadOnlyInspector] [SerializeField] private float calculationTimeMilliseconds;
    [ReadOnlyInspector] [SerializeField] private float calculationTimeSeconds;

    private void Start()
    {
        thisPosition = transform.position;
        CacheNewMapData();
    }

    public void CacheNewMapData()
    {
        if (blackboard == null) blackboard = GetComponent<AI_Blackboard>();
        if (pathVisualizer == null) pathVisualizer = GetComponent<AI_PathVisuals>();

        traversableVoxels = blackboard.GetAllTraversableVoxels();
        voxelGridPositions = blackboard.GetVoxelPositions();
        mapDimensions = blackboard.GetMapDimensions();
        voxelSize = blackboard.GetVoxelSize();
    }

    public void CalculatePath(Vector3 pAgentPosition, Vector3 pTargetPosition, out List<Vector3> pPathPositions)
    {
        float startTime = Time.realtimeSinceStartup;

        Vector3Int agentPos = Vector3Int.RoundToInt(pAgentPosition);
        Vector3Int targetPos = Vector3Int.RoundToInt(pTargetPosition);
        VoxelContainer startVoxel = getVoxelFromWorldPosition(agentPos);
        VoxelContainer targetVoxel = getVoxelFromWorldPosition(targetPos);

        Debug.Assert(startVoxel != null,
            "StartVoxel is null. Make sure the Transform is inside the grid, and is not in a collider (red) voxel.");
        Debug.Assert(targetVoxel != null,
            "TargetVoxel is null. Make sure the Transform is inside the grid, and is not in a collider (red) voxel.");

        calculateAllVoxelCosts(startVoxel, targetVoxel);

        blackboard.path = null;
        blackboard.targetVoxel = targetVoxel;
        blackboard.startVoxel = startVoxel;
        VoxelContainer currentVoxel = null;

        Dictionary<int, VoxelContainer> openVoxels = new Dictionary<int, VoxelContainer>();
        Dictionary<int, VoxelContainer> closedVoxels = new Dictionary<int, VoxelContainer>();
        VoxelContainer lastVoxel = startVoxel;

        openVoxels.Add(startVoxel.ID, startVoxel);

        while (openVoxels.Count > 0)
        {
            var sortedFcosts = openVoxels.OrderBy(d => d.Value.Fcost).ToList();
            currentVoxel = sortedFcosts[0].Value;

            if (openVoxels.ContainsKey(currentVoxel.ID))
                openVoxels.Remove(currentVoxel.ID);

            if (!closedVoxels.ContainsKey(currentVoxel.ID))
                closedVoxels.Add(currentVoxel.ID, currentVoxel);

            if (currentVoxel == targetVoxel)
                break;

            Dictionary<int, VoxelContainer> neighbourVoxels = linkIDtoNeighbours(currentVoxel);
            if (neighbourVoxels.Count == 0)
            {
                pPathPositions = null;
                return;
            }

            compareNeighbours(neighbourVoxels, closedVoxels, openVoxels, currentVoxel, lastVoxel);

            lastVoxel = currentVoxel;
        }

        List<VoxelContainer> pathVoxels = retracePath(startVoxel, targetVoxel);
        pPathPositions = getPathPositions(pathVoxels);

        blackboard.path = pathVoxels;

        closedVoxels.Clear();
        openVoxels.Clear();

        calculationTimeSeconds = (Time.realtimeSinceStartup - startTime);
        calculationTimeMilliseconds = (Time.realtimeSinceStartup - startTime) * 1000f;
    }

    private VoxelContainer getVoxelFromWorldPosition(Vector3Int pPosition)
    {
        voxelGridPositions.TryGetValue(pPosition, out int id);
        traversableVoxels.TryGetValue(id, out VoxelContainer voxel);
        return voxel;
    }

    private void compareNeighbours(Dictionary<int, VoxelContainer> pNeighbourVoxels,
        Dictionary<int, VoxelContainer> pClosedVoxels, Dictionary<int, VoxelContainer> pOpenVoxels,
        VoxelContainer pCurrentVoxel, VoxelContainer pLastVoxel)
    {
        foreach (KeyValuePair<int, VoxelContainer> voxel in pNeighbourVoxels)
        {
            if (voxel.Value.ID == pLastVoxel.ID)
                continue;
            if (pClosedVoxels.ContainsKey(voxel.Value.ID))
                continue;

            NativeArray<int> newGCostContainer = new NativeArray<int>(1, Allocator.TempJob);

            AStarDistanceJob distanceJob = new AStarDistanceJob()
            {
                Voxel1Position = pCurrentVoxel.WorldPosition,
                Voxel2Position = voxel.Value.WorldPosition,
                CurrentVoxelGCost = pCurrentVoxel.Gcost,
                NewGCostOutput = newGCostContainer
            };
            JobHandle jobHandle = distanceJob.Schedule();
            jobHandle.Complete();

            if (newGCostContainer[0] < voxel.Value.Gcost)
            {
                voxel.Value.Parent = pCurrentVoxel;
                voxel.Value.Gcost = newGCostContainer[0];
                voxel.Value.Fcost = voxel.Value.Gcost + voxel.Value.Hcost;
            }

            newGCostContainer.Dispose();

            if (!pOpenVoxels.ContainsKey(voxel.Value.ID))
                pOpenVoxels.Add(voxel.Value.ID, voxel.Value);
        }
    }

    private Dictionary<int, VoxelContainer> linkIDtoNeighbours(VoxelContainer pCurrentVoxel)
    {
        Dictionary<int, VoxelContainer> neighbourVoxels = new Dictionary<int, VoxelContainer>();

        if (pCurrentVoxel == null) return neighbourVoxels;
        if (pCurrentVoxel.NeighbourVoxelIDs == null) return neighbourVoxels;

        int[] neighbourIDs = pCurrentVoxel.NeighbourVoxelIDs.ToArray();

        for (int i = 0; i < neighbourIDs.Length; i++)
        {
            //Linking IDs to voxel instances.
            traversableVoxels.TryGetValue(neighbourIDs[i], out var neighbour);
            if (neighbour != null) neighbourVoxels.Add(neighbour.ID, neighbour);
        }

        return neighbourVoxels;
    }

    private List<VoxelContainer> retracePath(VoxelContainer pStartVoxel, VoxelContainer pEndVoxel)
    {
        List<VoxelContainer> path = new List<VoxelContainer> {pEndVoxel};

        VoxelContainer currentRetracePosition = pEndVoxel;

        while (currentRetracePosition != pStartVoxel && currentRetracePosition.Parent != null)
        {
            path.Add(currentRetracePosition);
            currentRetracePosition = currentRetracePosition.Parent;
        }

        path.Reverse();
        return path;
    }

    private List<Vector3> getPathPositions(List<VoxelContainer> pPathVoxels)
    {
        List<Vector3> pPathPositions = new List<Vector3>();

        for (int i = 0; i < pPathVoxels.Count; i++)
        {
            pPathPositions.Add(pPathVoxels[i].WorldPosition);
        }

        return pPathPositions;
    }


    private void calculateAllVoxelCosts(VoxelContainer pStartVoxel, VoxelContainer pTargetVoxel)
    {
        NativeList<float3> voxelPositions = new NativeList<float3>(Allocator.TempJob);
        NativeArray<int> gCostContainer = new NativeArray<int>(traversableVoxels.Count, Allocator.TempJob);
        NativeArray<int> hCostContainer = new NativeArray<int>(traversableVoxels.Count, Allocator.TempJob);
        NativeArray<int> fCostContainer = new NativeArray<int>(traversableVoxels.Count, Allocator.TempJob);

        foreach (KeyValuePair<int, VoxelContainer> voxel in traversableVoxels)
        {
            voxelPositions.Add(voxel.Value.WorldPosition);
        }

        CostCalculationJob job = new CostCalculationJob()
        {
            StartVoxelPos = pStartVoxel.WorldPosition,
            TargetVoxelPos = pTargetVoxel.WorldPosition,
            AllVoxelPos = voxelPositions,
            GCostOutput = gCostContainer,
            FCostOutput = fCostContainer,
            HCostOutput = hCostContainer
        };

        JobHandle handle = job.Schedule(traversableVoxels.Count, 200);
        handle.Complete();

        int index = 0;
        foreach (KeyValuePair<int, VoxelContainer> voxel in traversableVoxels)
        {
            voxel.Value.Gcost = gCostContainer[index];
            voxel.Value.Fcost = fCostContainer[index];
            voxel.Value.Hcost = hCostContainer[index];
            index++;
        }

        voxelPositions.Dispose();
        gCostContainer.Dispose();
        hCostContainer.Dispose();
        fCostContainer.Dispose();
    }
}