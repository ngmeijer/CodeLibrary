using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    #region Variables

    //------------------------------------------------------------------//
    [Header("Input keys")]
    [SerializeField] public KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] public KeyCode jumpKey = KeyCode.Space;
    [SerializeField] public KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] public KeyCode reverseViewKey = KeyCode.Q;
    [SerializeField] public KeyCode dashKey = KeyCode.CapsLock;
    [SerializeField] public KeyCode activateInventoryKey = KeyCode.Tab;
    [SerializeField] public KeyCode explosionAbilityKey = KeyCode.E;

    //------------------------------------------------------------------//

    //Before I remove all [SerializeFields], is there any other way of accessiblity instead of public? Thought protected but it only allows it for parents..

    //------------------------------------------------------------------//
    [Header("Movement values")]
    [SerializeField] public float walkSpeed = 4f;
    [SerializeField] public float sprintSpeed = 8f;
    [SerializeField] public float speedIncrease = 1;

    public float moveSpeed = 0f;

    [SerializeField] public float rotateSpeed = 100f;

    [SerializeField] public float jumpForce = 100f;

    [SerializeField] public float boostPower = 5f;

    [SerializeField] public float downForce = -1f;

    [SerializeField] public float forceRadius = 1f;
    [SerializeField] public float explosionForce = 500f;
    [SerializeField] public float maxExplosionForce = 1500f;
    [SerializeField] public float maxTimeAlive;

    [SerializeField] public Vector3 camNormalPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] public Vector3 camCrouchPosition = new Vector3(0f, -0.8f, 0f);
    [SerializeField] public Quaternion reverseViewRotation = new Quaternion(0f, 180f, 0f, 0f);
    [SerializeField] public Quaternion normalViewRotation = new Quaternion(0f, 0f, 0f, 0f);
    //------------------------------------------------------------------//

    #endregion
}
