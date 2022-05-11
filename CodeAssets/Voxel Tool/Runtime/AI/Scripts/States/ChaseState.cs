
using System.Collections;
using AiStates;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : AI_State, IAgentPathFunctions
{
    private float Radius;

    public override IEnumerator EnterState()
    {
        yield return null;
        
        if (isPropertiesNull)
        {
            Debug.Log("No AI_ScrObject Properties valid reference in PatrolState.");
            yield break;
        }
            
        if (isAgentNull)
        {
            Debug.Log("No NavMeshAgent valid reference in PatrolState.");
            yield break;
        }

        CurrentPath = new NavMeshPath();
        Radius = properties.MaxSight * 2;
        agent.speed = properties.ChaseSpeed;
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
        if (!agent.isOnNavMesh)
        {
            Debug.Log("NavMesh is not valid.");
            yield break;
        }
        if (Target == null) yield break;
        if (!shouldUpdate) yield break;

        yield return new WaitForSeconds(properties.PathCalculationInterval);

        if (NavMesh.SamplePosition(Target.transform.position, out NavMeshHit hit, 20f, 1))
        {
            Debug.Log("found sample point");
            if (agent.CalculatePath(hit.position, CurrentPath))
            {
                if (CurrentPath.status == NavMeshPathStatus.PathComplete)
                    agent.SetPath(CurrentPath);
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