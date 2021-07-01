using System;
using System.Collections;
using UnityEngine;

namespace AiStates
{
    public class AttackState : AI_State
    {
        private float Radius;
        [SerializeField] private MeshCollider attackCollider;

        public override void EnterState()
        {
            if (isPropertiesNull)
            {
                Debug.Log("No AI_ScrObject Properties valid reference in AttackState.");
                return;
            }
            
            Radius = Properties.AttackRange * 2;
            Agent.isStopped = true;

            shouldUpdate = true;
            StartCoroutine(handleAttackAction());
        }

        protected override void Update()
        {
            if (!shouldUpdate) return;
            
            if (checkIfDistanceToTargetIsTooHigh(Radius))
                ExitState();
        }

        private IEnumerator handleAttackAction()
        {
            if (!shouldUpdate) yield break;
            
            
            
            yield return new WaitForSeconds(Properties.AttackSpeed);
            Debug.Log("Attacking.");

            StartCoroutine(handleAttackAction());
        }

        public override void ExitState()
        {
            Agent.isStopped = false;
            shouldUpdate = false;
            controller.ReceiveStateExit();
        }
    }
}