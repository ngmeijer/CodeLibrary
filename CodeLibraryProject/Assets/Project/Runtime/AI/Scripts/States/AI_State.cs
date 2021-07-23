using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AiStates
{
    public enum FSMStates
    {
        IDLE,
        PATROL,
        CHASE,
        ATTACK,
        BORED,
    };

    public abstract class AI_State : MonoBehaviour
    {
        [HideInInspector] public GameObject Target;
        public GameObject ParentGameObject;
        protected Transform parentTransform;
        protected Vector3 parentPosition;
        protected AI_ScrObject properties;

        protected NavMeshAgent agent;
        public NavMeshPath CurrentPath;
        protected SkunkController controller;
        
        protected bool shouldUpdate;
        protected bool isAgentNull;
        protected bool isPropertiesNull;
                
        private void Awake()
        {
            isPropertiesNull = properties == null;
            isAgentNull = agent == null;
            controller = GetComponentInParent<SkunkController>();
            agent = controller.Agent;
            properties = controller.Properties;
            ParentGameObject = transform.parent.gameObject;
            parentTransform = ParentGameObject.transform;
            parentPosition = parentTransform.position;
        }

        public abstract IEnumerator EnterState();
        
        protected abstract void Update();

        public abstract void ExitState();
        
        protected bool checkIfDistanceToTargetIsTooHigh(float pRadius)
        {
            float distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
            if (distanceToTarget > pRadius)
                return true;

            return false;
        }
    }
}

public interface IAgentPathFunctions
{
    public IEnumerator SetPath();
}