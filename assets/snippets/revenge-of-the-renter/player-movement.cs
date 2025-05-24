using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables
    // Movement speeds
    public float walkSpeed = 12f;
    public float normalSpeed;
    public float crouchSpeed;

    // Jumping parameters
    public float jumpPower = 7f;
    private bool isJumping = false;
    public float gravity = -9.81f;

    // Height parameters
    public float normalHeight, crouchHeight;

    // Private variables for movement
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;

    // Static variable checks
    static bool isCrouching = false;
    static public bool canMove = true;
    static public bool playerActivated = true;

    #endregion

    void Start()
    {
        // Get reference to CharacterController component
        characterController = GetComponent<CharacterController>();

        // Set variables
        crouchSpeed = walkSpeed / 2;
        normalSpeed = walkSpeed;
        normalHeight = characterController.height;
        crouchHeight = normalHeight / 2;
    }

    void Update()
    {
        #region canMove Code
        if (!playerActivated || PlayerHealth.isDead || PauseMenu.GameIsPaused || RCmovement.rcActivated)
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }
        #endregion

        #region Player Movement
        // Get the direction of movement based on player's input
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Calculate movement speed based on input
        float curSpeedX = canMove ? walkSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? walkSpeed * Input.GetAxis("Horizontal") : 0;

        // Store the current vertical movement direction
        float movementDirectionY = moveDirection.y;

        // Combine the movement directions
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        #region Handle crouching
        if (Input.GetKeyDown(KeyCode.LeftShift) && canMove && !isJumping)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            characterController.height = normalHeight;
            walkSpeed = normalSpeed;
            isCrouching = false;
        }
        #endregion

        #region Handle jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded && !isCrouching)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        #endregion

        //Apply Gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y += gravity * Time.deltaTime;
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }

        // Move the player
        characterController.Move(moveDirection * Time.deltaTime);
        #endregion
    }
}