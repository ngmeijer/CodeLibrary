using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The main point of this class is to reduce the load on the system & organize the path requests.
/// If there are 10 agents in the scene (best case scenario),
/// and they all need a new path every 3 seconds, it will result in very high lag spikes > unplayable game.
/// Therefore, a queue is needed (first in, first out) to serve the agent that has been waiting the longest.
/// </summary>
[ExecuteAlways]
public class PathQueue : MonoBehaviour
{
    [Tooltip("How often does the system throw out a new path? " +
             "If an agent does not have a path, it will stand idle until a path has been received.")]
    [SerializeField] [Range(0.01f, 1f)] private float queueInterval;
    [SerializeField] private AI_PathFinder pathFinder;

    private Queue<AI_Agent> agentQueue = new Queue<AI_Agent>();
    private Queue<QueueInputData> agentInputData = new Queue<QueueInputData>();
    private float queueTimer = 0f;
    private List<Vector3> agentNewPath = new List<Vector3>();
    private bool isPathFinderNull;

    private void Awake()
    {
        isPathFinderNull = pathFinder == null;
        if (isPathFinderNull) pathFinder = FindObjectOfType<AI_PathFinder>();
    }

    private IEnumerator Start()
    {
        yield return null;
        
        if(agentQueue.Count > 0)
            takeDataFromQueues();
    }

    private void Update()
    {
        if (agentQueue.Count == 0)
            return;
        queueTimer += Time.deltaTime;
        if (queueTimer > queueInterval)
        {
            takeDataFromQueues();
        }
    }

    private void takeDataFromQueues()
    {
        AI_Agent currentAgent = agentQueue.Dequeue();
        QueueInputData currentData = agentInputData.Dequeue();
            
        requestPathForAgent(currentAgent, currentData);
        queueTimer = 0;
    }
    

    private void requestPathForAgent(AI_Agent pAgent, QueueInputData pData)
    {
        if (isPathFinderNull) return;
        pathFinder.CalculatePath(pData.startPosition, pData.targetPosition, out agentNewPath);
        pAgent.ReceivePath(agentNewPath);
    }

    public void AddToWaitingLine(AI_Agent pAgent, QueueInputData pData)
    {
        agentQueue.Enqueue(pAgent);
        agentInputData.Enqueue(pData);
    }
}