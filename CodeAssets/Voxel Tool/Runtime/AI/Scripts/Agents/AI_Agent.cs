using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AI_Agent : MonoBehaviour
{
    [SerializeField] private AI_ScrObject agentFile;
    [SerializeField] private Vector3 moveAreaSize;
    [SerializeField] private bool showMoveArea;

    private List<Vector3> currentPathPositions;
    private List<Transform> targetCollection;
    private int waypointIndex;
    private int lastIndex;
    private Vector3 currentWaypoint;
    private Vector3 targetWaypoint;
    private bool shouldMove;
    private PathQueue queue;
    private AI_Blackboard blackboard;

    private void Awake()
    {
        queue = FindObjectOfType<PathQueue>();
        blackboard = FindObjectOfType<AI_Blackboard>();
    }

    private void Start()
    {
        targetCollection = blackboard.getPossibleWaypoints();
        RequestNewPath();
    }

    private void Update()
    {
        if (!shouldMove)
            return;

        if (currentPathPositions.Count <= 1)
            return;

        float step = agentFile.NormalMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, step);
        Vector3 targetDirection = currentWaypoint - transform.position;

        float singleStep = agentFile.NormalMoveSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        if (transform.position == currentWaypoint && currentWaypoint != targetWaypoint)
        {
            waypointIndex++;
            currentWaypoint = currentPathPositions[waypointIndex];
        }

        if (transform.position == targetWaypoint)
        {
            shouldMove = false;
            RequestNewPath();
        }
    }

    public void RequestNewPath()
    {
        QueueInputData data = new QueueInputData()
        {
            startPosition = transform.position,
            targetPosition = getRandomTargetPoint(),
        };
        queue.AddToWaitingLine(this, data);
    }

    private Vector3 getRandomTargetPoint()
    {
        int randomIndex = -1;
        while (randomIndex == lastIndex || randomIndex == -1)
            randomIndex = Random.Range(0, targetCollection.Count);

        lastIndex = randomIndex;

        return targetCollection[randomIndex].position;
    }

    public void ReceivePath(List<Vector3> pPositions)
    {
        currentPathPositions = pPositions;
        waypointIndex = 0;

        if (currentPathPositions == null)
        {
            Debug.Log("Received path is null.");
            return;
        }

        if (currentPathPositions.Count == 0)
        {
            Debug.Log("Received path didn't contain any positions..");
            return;
        }

        currentWaypoint = currentPathPositions[waypointIndex];
        targetWaypoint = currentPathPositions[currentPathPositions.Count - 1];
        shouldMove = true;
    }

    private void OnDrawGizmos()
    {
        if (showMoveArea)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, moveAreaSize);
        }
    }
}

public struct QueueInputData
{
    public Vector3 startPosition;
    public Vector3 targetPosition;
}