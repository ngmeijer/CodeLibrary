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
            agent.speed = properties.NormalMoveSpeed;
            
            parentTransform = ParentGameObject.transform;
            parentPosition = parentTransform.position;
            
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
            if (!agent.isOnNavMesh)
            {
                Debug.Log("NavMesh is not valid.");
                yield break;
            }
            if (agent.hasPath) yield break;
            
            while (randomPatrolPoint == Vector3.zero)
            {
                randomPatrolPoint = getRandomTarget();
            }
            
            //Calculates the best path for the point
            if (agent.CalculatePath(randomPatrolPoint, CurrentPath))
            {
                if (CurrentPath.status != NavMeshPathStatus.PathInvalid && CurrentPath.status != NavMeshPathStatus.PathPartial) 
                    agent.SetPath(CurrentPath);
            }
            //No path available? Get new point
            else randomPatrolPoint = getRandomTarget();
        }

        private Vector3 getRandomTarget()
        {
            float randomMultiplier = Random.Range(properties.MinWalkRange, properties.MaxWalkRange);
            Vector3 randomDirection = Random.insideUnitSphere * randomMultiplier;
            
            //Get the rough relative direction to go in.
            randomDirection += ParentGameObject.transform.position;

            NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, properties.MaxWalkRange, 1);
            float distanceToCandidatePoint = Vector3.Distance(parentPosition, hit.position);
            if (distanceToCandidatePoint < properties.MinWalkRange)
                return Vector3.zero;
            
            return hit.position;
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