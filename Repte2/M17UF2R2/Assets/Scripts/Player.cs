using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Player : MonoBehaviour
{
    // Animation variables
    [SerializeField] float animationSmoothTime = 0.5f;
    Animator animator;
    private PlayerControls playerControls;

    int moveXAnimationParamenterID;
    int moveZAnimationParamenterID;
    int magnitudeAnimationParamenterID;
    int isSprintingAnimationParamenterID;
    int isMovingnimationParamenterID;
    int isJumpingParamenterID;
    int isGroundedParamenterID;

    [SerializeField] bool isMoving = false;
    [SerializeField] bool isSprinting = false;
    [SerializeField] bool isJumping = false;
    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    InputAction.CallbackContext context;
    Vector2 input;


    // Movement variables
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 2f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 2f;


    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
        
    }

    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
        //Inputs
        playerControls = new PlayerControls();
        playerControls.Player.Jump.performed += GetJumpInputs;
        playerControls.Player.MoveStart.performed += GetMovementInputs;
        playerControls.Player.SprintStart.performed += context => GetSprintInput(context, true);
        playerControls.Player.SprintFinish.performed += context => GetSprintInput(context, false);
        //Animations
        animator = GetComponent<Animator>();
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
        input = context.ReadValue<Vector2>();
        isMoving = input.x != 0 || input.y != 0;
        groundedPlayer = controller.isGrounded;

        Animate();
        Move();
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


        if (!isSprinting) animator.SetFloat(magnitudeAnimationParamenterID, currentAnimationBlendVector.magnitude);

        //Rotate towards camera direction
        Quaternion rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = rotation;
        //Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

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
        move.y = 0f;

        if (isSprinting) playerSpeed = 4f;
        else playerSpeed = 2f;
        controller.Move(playerSpeed  * Time.deltaTime * move);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

    }
    private void GetSprintInput(InputAction.CallbackContext context, bool isSprint)
    {
        isSprinting = isMoving && isSprint;
        float magnitude = isSprinting ? context.ReadValue<float>() + 1.0f : 0.0f;
        animator.SetFloat(magnitudeAnimationParamenterID, magnitude);
        animator.SetBool(isSprintingAnimationParamenterID, isSprinting);
    }
    private void GetMovementInputs(InputAction.CallbackContext context)
    {
        this.context = context;
    }
    private void GetJumpInputs(InputAction.CallbackContext context)
    {
        if (groundedPlayer)
        {
            isJumping = true;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }
          
    }





}
