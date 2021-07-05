using System;
using System.Collections;
using UnityEngine;

namespace AiStates
{
    public class AttackState : AI_State
    {
        private float Radius;

        public override void EnterState()
        {
            if (isPropertiesNull)
            {
                Debug.Log("No AI_ScrObject Properties valid reference in AttackState.");
                return;
            }
            
            Radius = properties.AttackRange * 2;
            agent.isStopped = true;

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
            
            yield return new WaitForSeconds(properties.AttackSpeed);
            Debug.Log("Attacking.");

            StartCoroutine(handleAttackAction());
        }

        public override void ExitState()
        {
            agent.isStopped = false;
            shouldUpdate = false;
            controller.ReceiveStateExit();
        }
    }
}