using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RCmovement : MonoBehaviour
{
    // Movement speeds
    public float walkSpeed = 8f;

    // Jumping parameters
    public float gravity = -9.81f;

    // Private variables for movement
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    static public bool rcActivated = false;
    static public bool rcCanMove = false;

    void Start()
    {
        // Get reference to CharacterController component
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        #region rcActivated Code
        if (PlayerHealth.isDead || PlayerMovement.playerActivated)
        {
            rcActivated = false;
        }
        #endregion

        #region rcCanMove Code
        if (!rcActivated || PlayerHealth.isDead || PauseMenu.GameIsPaused) 
        { 
            rcCanMove = false; 
        }
        else if (rcActivated && (!PauseMenu.GameIsPaused || !PlayerMovement.playerActivated))
        {
            rcCanMove = true;
        }
        #endregion

        #region rcMovement Code
        if (rcActivated && rcCanMove && !PlayerMovement.canMove)
        {
            // Get the direction of movement based on input
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            // Calculate movement speed based on input
            float curSpeedX = rcActivated ? walkSpeed * Input.GetAxis("Vertical") : 0;

            // Store the current vertical movement direction
            float movementDirectionY = moveDirection.y;

            // Combine the movement directions
            moveDirection = (forward * curSpeedX);

            // Apply gravity
            if (!characterController.isGrounded)
            {
                moveDirection.y += gravity * Time.deltaTime;
            }

            // Move the RC
            characterController.Move(moveDirection * Time.deltaTime);
        }
        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (rcActivated && rcCanMove)
        {
            if (other.gameObject.CompareTag("Landlord"))
            {
                Debug.Log("Collision detected with " + other.gameObject.name); // Log the collision object
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("RcChargingStation"))
        {
            RCBattery.remainingTime += 0.03f;
        }
    }
}