using System;
using System.Collections;
using UnityEngine;

namespace AiStates
{
    public class IdleState : AI_State
    {
        public override IEnumerator EnterState()
        {
            yield return null;
            shouldUpdate = true;
        }

        protected override void Update()
        {
            if (!shouldUpdate) return;
            
            Debug.Log("Running update in IdleState");
        }

        public override void ExitState()
        {
            shouldUpdate = false;

        }
    }
}