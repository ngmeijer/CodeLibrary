using System.Collections;
using System.Collections.Generic;
using AiStates;
using UnityEngine;

public class SkunkAnimationHandler : MonoBehaviour
{
    private string currentTrigger;
    [SerializeField] private Animator anim;
    private float chasingAnimationSpeed = 2.5f;
    private float walkingAnimationSpeed = 1.3f;
    
    public void ReceiveStateSwitchNotify(int pStateInt)
    {
        FSMStates convertedState = (FSMStates) pStateInt;

        anim.speed = 1;

        switch (convertedState)
        {
            case FSMStates.IDLE:
                currentTrigger = "Idle";
                break;
            case FSMStates.CHASE:
                currentTrigger = "Chase";
                anim.speed = chasingAnimationSpeed;
                break;
            case FSMStates.ATTACK:
                currentTrigger = "Attack";
                break;
            case FSMStates.PATROL:
                anim.speed = walkingAnimationSpeed;
                currentTrigger = "Walk";
                break;
        }
        
        triggerNewAnimation();
    }
    
    private void triggerNewAnimation()
    {
        anim.SetTrigger(currentTrigger);
    }
}
