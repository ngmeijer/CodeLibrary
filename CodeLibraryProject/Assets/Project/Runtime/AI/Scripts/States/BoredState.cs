using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiStates
{
    public class BoredState : AI_State
    {
        public override IEnumerator EnterState()
        {
            yield return null;
            //When the skunk hasn't interacted with the player for too long, it will search for the player and attempt to attack him.
            //Get player position
            
            shouldUpdate = true;
        }

        protected override void Update()
        {
            if (!shouldUpdate) return;
            
            Debug.Log("Running update in BoredState");
        }

        public override void ExitState()
        {            
            shouldUpdate = false;
        }
    }
}