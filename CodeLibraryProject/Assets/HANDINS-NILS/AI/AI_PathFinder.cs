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

[Serializable]
public class Event_OnPathCalculated : UnityEvent
{
}

[BurstCompile]
public class AI_PathFinder : MonoBehaviour
{
    private Dictionary<int, VoxelContainer> traversableVoxels = new Dictionary<int, VoxelContainer>();
    private float[] mapDimensions;
    private float voxelSize;
    private Vector3 thisPosition;
    [SerializeField] private AI_Blackboard blackboard;

    [SerializeField] [Range(5, 200)] private float maxTravelDistanceBetweenVoxels;
    [SerializeField] private Event_OnPathCalculated onPathCalculated;

    private void Start()
    {
        thisPosition = transform.position;
        CacheNewMapData();
    }

    public void CacheNewMapData()
    {
        if (blackboard == null)
        {
            Debug.LogError("AI_Blackboard reference is null.");
            return;
        }

        traversableVoxels = blackboard.GetAllTraversableVoxels();
        mapDimensions = blackboard.GetMapDimensions();
        voxelSize = blackboard.GetVoxelSize();
    }

    public void DeterminePathLength(Vector3 pAgentPosition, Vector3 pTargetPosition)
    {
        //Calculate distance between points
        float distance = Vector3.Distance(pAgentPosition, pTargetPosition);

        //Calculate if (and how many) divisions are necessary
        int divisions = (int) (distance / maxTravelDistanceBetweenVoxels);

        //Create temporary waypoints (interpolation?)
        List<Vector3> waypointList = new List<Vector3> { };
        for (float i = 1; i < divisions; i++)
        {
            Vector3 waypoint = Vector3.Lerp(pAgentPosition, pTargetPosition, i / divisions);
            waypointList.Add(waypoint);
        }
    }

    public void CalculatePath(Vector3 pAgentPosition, Vector3 pTargetPosition, out List<Vector3> pathPositions)
    {
        float startTime = Time.realtimeSinceStartup;

        if (traversableVoxels.Count == 0)
            CacheNewMapData();
        if (traversableVoxels.Count == 0)
        {
            pathPositions = null;
            return;
        }

        VoxelContainer startVoxel = VoxelPositionHandler.GetVoxelFromWorldPos(traversableVoxels, pAgentPosition,
            thisPosition, mapDimensions, voxelSize);
        VoxelContainer targetVoxel = VoxelPositionHandler.GetVoxelFromWorldPos(traversableVoxels, pTargetPosition,
            thisPosition, mapDimensions, voxelSize);

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
        int neighbourCounter = 0;
        int openVoxelCounter = 0;

        while (openVoxels.Count > 0)
        {
            openVoxelCounter++;
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
                pathPositions = null;
                return;
            }

            compareNeighbours(neighbourVoxels, closedVoxels, openVoxels, currentVoxel, lastVoxel);

            lastVoxel = currentVoxel;
        }

        List<VoxelContainer> pathVoxels = retracePath(startVoxel, targetVoxel);
        pathPositions = getPathPositions(pathVoxels);

        blackboard.path = pathVoxels;

        closedVoxels.Clear();
        openVoxels.Clear();

        // Debug.Log($"Pathfinding calculation took: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
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
                Voxel1Position = pCurrentVoxel.Position,
                Voxel2Position = voxel.Value.Position,
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
            pPathPositions.Add(pPathVoxels[i].Position);
        }

        return pPathPositions;
    }


    private void calculateAllVoxelCosts(VoxelContainer pStartVoxel, VoxelContainer pTargetVoxel)
    {
        NativeList<float3> VoxelPositions = new NativeList<float3>(Allocator.TempJob);
        NativeArray<int> gCostContainer = new NativeArray<int>(traversableVoxels.Count, Allocator.TempJob);
        NativeArray<int> hCostContainer = new NativeArray<int>(traversableVoxels.Count, Allocator.TempJob);
        NativeArray<int> fCostContainer = new NativeArray<int>(traversableVoxels.Count, Allocator.TempJob);

        foreach (KeyValuePair<int, VoxelContainer> voxel in traversableVoxels)
        {
            VoxelPositions.Add(voxel.Value.Position);
        }
        
        CostCalculationJob job = new CostCalculationJob()
        {
            StartVoxelPos = pStartVoxel.Position,
            TargetVoxelPos = pTargetVoxel.Position,
            AllVoxelPos = VoxelPositions,
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

        VoxelPositions.Dispose();
        gCostContainer.Dispose();
        hCostContainer.Dispose();
        fCostContainer.Dispose();
    }

    private IEnumerator StartDelay()
    {
        yield return null;
    }
}