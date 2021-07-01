using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prototyping.AI.Scripts;
using UnityEngine;
using AiStates;
using Unity.Collections;
using UnityEngine.AI;

public class SkunkController : MonoBehaviour
{
    [SerializeField] private AI_ScrObject dataFile;
    [SerializeField] private Transform rayCastOrigin;

    [ReadOnly] public AI_State CurrentState;

    [Header("FSM States")] 
    [SerializeField] private AttackState attackState;
    [SerializeField] private PatrolState patrolState;
    [SerializeField] private ChaseState chaseState;
    [SerializeField] private BoredState boredState;
    [SerializeField] private IdleState idleState;

    [SerializeField]
    private SerializableDictionary<GameObject, float> targetUtilities = new SerializableDictionary<GameObject, float>();

    private SkunkAnimationHandler animationHandler;
    private NavMeshAgent agent;
    private float interactionTimer;
    private float distanceToPlayer;
    private bool hasTarget;
    private List<GameObject> targets;
    private IOrderedEnumerable<KeyValuePair<GameObject, float>> sortedUtilities;
    private KeyValuePair<GameObject, float> priorityTarget;
    private KeyValuePair<GameObject, float> previousTarget;
    private GUIStyle utilityGUIStyle;
    private GUIStyle generalGUIStyle;
    private GUIStyle stateGUIStyle;

    private void Start()
    {
        animationHandler = GetComponent<SkunkAnimationHandler>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = dataFile.NormalMoveSpeed;

        defineGizmoGUIStyles();
        findTargets();
        handlePatrolStateSwitch();

        StartCoroutine(checkTargetDistance());
    }

    private void defineGizmoGUIStyles()
    {
        generalGUIStyle = new GUIStyle {fontSize = 30, normal = {textColor = Color.white}};
        utilityGUIStyle = new GUIStyle {fontSize = 20, normal = {textColor = Color.red}};
        stateGUIStyle = new GUIStyle {fontSize = 30, normal = {textColor = Color.cyan}};
    }

    private void findTargets()
    {
        targets = GameObject.FindGameObjectsWithTag("Bee_NPC").ToList();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) targets.Add(player);

        for (int i = 0; i < targets.Count; i++)
        {
            targetUtilities.Add(targets[i], 0);
        }
    }

    private IEnumerator checkTargetDistance()
    {
        yield return new WaitForSeconds(dataFile.UtilityInterval);

        for (int i = 0; i < targets.Count; i++)
        {
            float distanceToAgent = Vector3.Distance(rayCastOrigin.position, targets[i].transform.position);

            //Skip utility value calculation if target is out of sightRange.
            if (distanceToAgent > dataFile.MaxSight)
                continue;

            //Calculate utility values for all valid targets here.
            targetUtilities[targets[i]] = calculateUtility(targets[i].transform.position);
        }

        sortedUtilities = targetUtilities.OrderByDescending(d => d.Value);

        priorityTarget = sortedUtilities.First();

        if (priorityTarget.Value != 0)
        {
            enterNewState();
        }

        StartCoroutine(checkTargetDistance());
    }

    private void enterNewState()
    {
        float distanceToPriorityTarget = Vector3.Distance(transform.position, priorityTarget.Key.transform.position);

        //Inside attacking range
        if (distanceToPriorityTarget < dataFile.AttackRange)
        {
            handleAttackStateSwitch();
            return;
        }

        //Outside attacking range, but inside sight range.
        if (distanceToPriorityTarget < dataFile.MaxSight)
        {
            handleChaseStateSwitch();
            return;
        }

        //Outside sight range (does not see enemy agent)
        if (CurrentState != patrolState) handlePatrolStateSwitch();
    }

    private void handleChaseStateSwitch()
    {
        if (CurrentState == chaseState) return;

        agent.speed = dataFile.ChaseSpeed;
        chaseState.Target = priorityTarget.Key;
        handleStateSwitch(chaseState);
    }

    private void handleAttackStateSwitch()
    {
        if (CurrentState == attackState) return;

        attackState.Target = priorityTarget.Key;
        handleStateSwitch(attackState);
    }

    private void handlePatrolStateSwitch()
    {
        agent.speed = dataFile.NormalMoveSpeed;
        handleStateSwitch(patrolState);
    }

    private void handleStateSwitch(AI_State pState)
    {
        if (CurrentState != null) CurrentState.ExitState();
    
        CurrentState = pState;
        CurrentState.EnterState();
    }

    public void ReceiveStateExit()=> CurrentState = null;

    private float calculateUtility(Vector3 pTargetPosition)
    {
        //Don't shoot out rays in every possible direction all the time... instead, gather a list of NPCs/targets ONCE, check distance, if in range, calculate Utility to determine most important target.
        return Utility.Calculate(dataFile, rayCastOrigin.position, pTargetPosition);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = dataFile.SightVisuals;
        Gizmos.DrawSphere(transform.position, dataFile.MaxSight);
        UnityEditor.Handles.Label(transform.position + (transform.right * -dataFile.MaxSight), $"Sight: {dataFile.MaxSight}m");

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dataFile.AttackRange);
        UnityEditor.Handles.Label(transform.position + (transform.right * -dataFile.AttackRange),
            $"AttackRange: {dataFile.AttackRange}m");

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, dataFile.MaxWalkRange);
        UnityEditor.Handles.Label(transform.position + (transform.right * -dataFile.MaxWalkRange),
            $"MaxWalkRange: {dataFile.MaxWalkRange}m");

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dataFile.MinWalkRange);
        UnityEditor.Handles.Label(transform.position + (transform.right * -dataFile.MinWalkRange),
            $"MinWalkRange: {dataFile.MinWalkRange}m");

        if (CurrentState != null) 
        {
            UnityEditor.Handles.Label(transform.position, $"{CurrentState}", stateGUIStyle);
        
            if (CurrentState == chaseState || CurrentState == patrolState)
            {
                Gizmos.color = Color.magenta;
                foreach (Vector3 corner in CurrentState.CurrentPath.corners)
                {
                    Gizmos.DrawSphere(corner, 2f);
                }

                for (int i = 0; i < CurrentState.CurrentPath.corners.Length - 1; i++)
                    Debug.DrawLine(CurrentState.CurrentPath.corners[i], CurrentState.CurrentPath.corners[i + 1],
                        Color.magenta);
            }
        }

        //Target highlight (utility)
        if (hasTarget)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(priorityTarget.Key.transform.position, new Vector3(3, 3, 3));
        }

        foreach (KeyValuePair<GameObject, float> entry in targetUtilities)
        {
            UnityEditor.Handles.Label(entry.Key.transform.position, $"Utility: {entry.Value}", utilityGUIStyle);
        }
    }
    #endif
}