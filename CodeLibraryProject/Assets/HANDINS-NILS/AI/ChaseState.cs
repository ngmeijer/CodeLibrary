
using System.Collections;
using AiStates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : AI_State, IAgentPathFunctions
{
    private float Radius;
    public Vector3 targetGroundPosition;

    public override void EnterState()
    {
        if (isPropertiesNull)
        {
            Debug.Log("No AI_ScrObject Properties valid reference in PatrolState.");
            return;
        }
            
        if (isAgentNull)
        {
            Debug.Log("No NavMeshAgent valid reference in PatrolState.");
            return;
        }

        CurrentPath = new NavMeshPath();
        Radius = Properties.MaxSight * 2;
        Agent.speed = Properties.ChaseSpeed;
        shouldUpdate = true;
        StartCoroutine(SetPath());
    }

    protected override void Update()
    {
        if (!shouldUpdate) return;

        if (checkIfDistanceToTargetIsTooHigh(Radius)) ExitState();
    }

    public IEnumerator SetPath()
    {
        if (isAgentNull) yield break;
        if (!Agent.isOnNavMesh)
        {
            Debug.Log("NavMesh is not valid.");
            yield break;
        }
        if (Target == null) yield break;
        if (!shouldUpdate) yield break;

        yield return new WaitForSeconds(Properties.PathCalculationInterval);

        if (NavMesh.SamplePosition(Target.transform.position, out NavMeshHit hit, 20f, 1))
        {
            Debug.Log("found sample point");
            if (Agent.CalculatePath(hit.position, CurrentPath))
            {
                if (CurrentPath.status == NavMeshPathStatus.PathComplete)
                {
                    targetGroundPosition = hit.position;
                    Agent.SetPath(CurrentPath);
                }
            }
        }

        StartCoroutine(SetPath());
    }

    public override void ExitState()
    {
        Debug.Log("Exit chase state");
        shouldUpdate = false;
        controller.ReceiveStateExit();
    }
}