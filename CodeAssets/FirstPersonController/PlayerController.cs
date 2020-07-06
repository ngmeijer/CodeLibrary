using UnityEngine;

[RequireComponent(typeof(PlayerSettings))]
[RequireComponent(typeof(PlayerAnimations))]
public class PlayerController : MonoBehaviour
{
    #region Variables

    //------------------------------------------------------------------//
    [Header("Player components")]

    private CharacterController charController = null;
    private Rigidbody rb = null;
    private SelfDestruct selfDestruct = null;

    [SerializeField] private Camera playerCamera = null;

    private PlayerSettings playerSettings = null;
    private PlayerAnimations playerAnims = null;

    [SerializeField] private Transform groundCheck = null;
    private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private Vector3 velocity;
    private float gravity = -9.81f;
    private bool isGrounded = true;
    //------------------------------------------------------------------//

    //Change this to a method.
    [HideInInspector] public int nextObjective = 0;
    //------------------------------------------------------------------//

    #endregion

    #region Default methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerSettings = GetComponent<PlayerSettings>();
        playerAnims = GetComponent<PlayerAnimations>();
        selfDestruct = GetComponent<SelfDestruct>();
        Cursor.visible = false;
    }

    private void Start()
    {
        nullChecks();
    }

    //Wrote this method to satisfy part of the "Excellent" criteria in the rubric.
    private void nullChecks()
    {
        Debug.Assert(rb != null, "The player's RigidBody cannot be found.");
        Debug.Assert(groundCheck != null, "The player's GroundCheck child-object is null. This means the player won't know if he's standing on the ground or not, so jumping is disabled.");
        Debug.Assert(playerCamera != null, "The player's Camera is null. You shouldn't be able to see shit.");
        Debug.Assert(playerSettings != null, "The PlayerSettings script is null. Check the GetComponent, or serialize it to show in the inspector and drag it in.");
        Debug.Assert(playerAnims != null, "The PlayerAnims script is null. Check the GetComponent, or serialize it to show in the inspector and drag it in.");
        Debug.Assert(selfDestruct != null, "The SelfDestrect script component is not attached to the Player GameObject.");
    }

    private void Update()
    {
        crouchPlayer();
        rotatePlayer();
    }

    private void FixedUpdate()
    {
        movePlayer();
        jumpPlayer();
    }

    #endregion

    #region Movement related methods

    private void movePlayer()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 movementVector = new Vector3(horizontalMove, 0.0f, verticalMove) * playerSettings.moveSpeed * Time.deltaTime;
        Vector3 newPosition = rb.position + rb.transform.TransformDirection(movementVector);

        rb.MovePosition(newPosition);

        //Movement for characterController (ASK AT ASSESSMENT WHAT'S BEST FOR FPS GAMES!)
        //Vector3 movementVector = transform.right * horizontalMove + transform.forward * verticalMove;
        //charController.Move(movementVector * playerSettings.moveSpeed * Time.deltaTime);

        //if (isGrounded && velocity.y < 0)
        //{
        //    velocity.y = playerSettings.downForce;
        //}


        if (Input.GetKey(playerSettings.sprintKey))
        {
            playerSettings.moveSpeed = playerSettings.sprintSpeed;
        }
        else
        {
            playerSettings.moveSpeed = playerSettings.walkSpeed;
        }


        if (verticalMove > 0)
        {
            playerAnims.handleRunAnimation();
        }
        else
        {
            playerAnims.handleIdleAnimation();
        }
    }

    private void jumpPlayer()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            if (Input.GetKey(playerSettings.jumpKey))
            {
                Vector3 jumpVector = new Vector3(0.0f, playerSettings.jumpForce, 0.0f) * Time.deltaTime;
                rb.AddForce(jumpVector);
            }
        }

        //CharacterController jumping
        //velocity.y += gravity * Time.deltaTime;

        ////charController.Move(velocity * Time.deltaTime);

        //if (Input.GetKey(playerSettings.jumpKey) && isGrounded)
        //{
        //    velocity.y = Mathf.Sqrt(playerSettings.jumpForce * playerSettings.downForce * gravity * Time.deltaTime);
        //}
    }

    private void crouchPlayer()
    {
        if (Input.GetKey(playerSettings.crouchKey))
        {
            playerCamera.transform.localPosition = playerSettings.camCrouchPosition;
            playerAnims.handleCrouchAnimation();
            //charController.height = 0.3f;
        }
        else
        {
            playerCamera.transform.localPosition = playerSettings.camNormalPosition;
            //charController.height = 0.8f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Death"))
        {
            selfDestruct.resetLevel = true;
        }
    }

    #endregion

    #region Camera related Methods

    private void rotatePlayer()
    {
        float mouseHorizontal = Input.GetAxis("Mouse X") * playerSettings.rotateSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseHorizontal);
    }

    #endregion
}
