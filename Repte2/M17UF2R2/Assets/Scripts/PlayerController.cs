using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Animation variables
    [SerializeField] float animationSmoothTime = 1f;
    Animator animator;

    int moveXAnimationParamenterID;
    int moveZAnimationParamenterID;
    int magnitudeAnimationParamenterID;
    int isSprintingAnimationParamenterID;
    int isMovingnimationParamenterID;
    int isJumpingParamenterID;
    int isGroundedParamenterID;

    // State Variables
    [SerializeField] bool isMoving = false;
    [SerializeField] bool isSprinting = false;
    [SerializeField] bool isJumping = false;
    [SerializeField] private bool groundedPlayer;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    // Movement variables
    private CharacterController controller;
    private Vector3 playerVelocity;
    private Transform cameraTransform;


    //Player Variables
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float magnitude = 1f;

    private Vector2 input;

    private InputManager inputManager;

    private void Start()
    {
        inputManager = InputManager.Instance;
    }
    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();

        //Animations
        animator = GetComponentInChildren<Animator>();
        moveXAnimationParamenterID = Animator.StringToHash("Velocity X");
        moveZAnimationParamenterID = Animator.StringToHash("Velocity Z");
        magnitudeAnimationParamenterID = Animator.StringToHash("Magnitude");
        isSprintingAnimationParamenterID = Animator.StringToHash("isSprinting");
        isMovingnimationParamenterID = Animator.StringToHash("isMoving");
        isJumpingParamenterID = Animator.StringToHash("isJumping");
        isGroundedParamenterID = Animator.StringToHash("isGrounded");
        cameraTransform = Camera.main.transform;

    }

    private void Update()
    {
       
        // Check current inputs 
        input = inputManager.GetPlayerMovement();
        CheckIfIsMoving();
        groundedPlayer = controller.isGrounded;
        magnitude = inputManager.PlayerStartedSprinting() + 1f;
        isSprinting = isMoving && magnitude > 1f;
 
        Move();
        Animate();
    }
    private void CheckIfIsMoving()
    {
        if (!(input.x != 0 || input.y != 0)) StartCoroutine(WaitForBoolToChange());
        else isMoving = true;
    }

    // We wait a little bit to change the bool to false to ensure if the player stopped moving 
    private IEnumerator WaitForBoolToChange()
    {
        StopCoroutine(WaitForBoolToChange());
        yield return new WaitForSeconds(0.1f);
        isMoving = false;
    }
    private void Move()
    {
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            isJumping = false;
        }
       
        Vector3 move = new(input.x, 0, input.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;

        move.y = playerVelocity.y += gravityValue * Time.deltaTime;
        
        playerSpeed = isSprinting ? 4f : 2f;

        controller.Move(playerSpeed * Time.deltaTime * move);

    }
    public void Jump()
    {
        if (groundedPlayer)
        {
            isJumping = true;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3f * gravityValue);
        }
    }

    void Animate()
    {
        if (isMoving) currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);
        else currentAnimationBlendVector = input;

        //Apply inputs in animator 
        animator.SetFloat(moveXAnimationParamenterID, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParamenterID, currentAnimationBlendVector.y);
        animator.SetBool(isMovingnimationParamenterID, isMoving);
        animator.SetBool(isGroundedParamenterID, groundedPlayer);
        animator.SetBool(isJumpingParamenterID, isJumping);

        animator.SetFloat(magnitudeAnimationParamenterID, magnitude);
        animator.SetBool(isSprintingAnimationParamenterID, isSprinting);


        if (!isSprinting) animator.SetFloat(magnitudeAnimationParamenterID, currentAnimationBlendVector.magnitude);

        //Rotate towards camera direction
        Quaternion rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);  
        
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

    }

}






