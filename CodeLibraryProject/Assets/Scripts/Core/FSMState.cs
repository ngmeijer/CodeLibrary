using UnityEngine;

public abstract class FSMState : MonoBehaviour
{
    public abstract void InitializeState();
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
}