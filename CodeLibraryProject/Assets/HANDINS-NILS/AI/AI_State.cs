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
        public FSMStates allStates;
        public GameObject Target;
        public GameObject Parent;
        public AI_ScrObject Properties;

        public NavMeshAgent Agent;
        public NavMeshPath CurrentPath;
        protected SkunkController controller;
        
        protected bool shouldUpdate;
        protected bool isAgentNull;
        protected bool isPropertiesNull;
                
        private void Start()
        {
            isPropertiesNull = Properties == null;
            isAgentNull = Agent == null;
            controller = GetComponentInParent<SkunkController>();
        }

        public abstract void EnterState();
        
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