using System.Collections;
using AiStates;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Prototyping.AI.Scripts
{
    public class PatrolState : AI_State, IAgentPathFunctions
    {
        private Vector3 randomPatrolPoint;

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
            Agent.speed = Properties.NormalMoveSpeed;
            shouldUpdate = true;
        }

        protected override void Update()
        {
            if (!shouldUpdate) return;

            StartCoroutine(SetPath());
        }

        public IEnumerator SetPath()
        {
            if (isAgentNull)
            {
                Debug.Log("Agent component is not assigned.");
                yield break;
            }
            if (!Agent.isOnNavMesh)
            {
                Debug.Log("NavMesh is not valid.");
                yield break;
            }
            if (Agent.hasPath) yield break;

            randomPatrolPoint = getRandomTarget();
            
            if (Agent.pathStatus == NavMeshPathStatus.PathComplete) {
                Agent.enabled = false;
                Agent.enabled = true;
            }

            if (Agent.CalculatePath(randomPatrolPoint, CurrentPath))
            {
                if (CurrentPath.status != NavMeshPathStatus.PathInvalid && CurrentPath.status != NavMeshPathStatus.PathPartial) 
                    Agent.SetPath(CurrentPath);
            }
            else randomPatrolPoint = getRandomTarget();
        }

        private Vector3 getRandomTarget()
        {
            float randomMultiplier = Random.Range(Properties.MinWalkRange, Properties.MaxWalkRange);
            Vector3 randomDirection = Random.insideUnitSphere * randomMultiplier;
            
            //Get the rough direction to go in.
            randomDirection += Parent.transform.position;
            NavMeshHit hit;
            
            //Find a suitable point on the NavMesh using the given rough direction.
            NavMesh.SamplePosition(randomDirection, out hit, Properties.MaxWalkRange, 1);
            Vector3 finalPosition = hit.position;

            return finalPosition;
        }

        public override void ExitState()
        {
            shouldUpdate = false;
            controller.ReceiveStateExit();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(randomPatrolPoint, 2f);
        }
    }
}