using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{

    [SerializeField] float animationSmoothTime = 0.5f;
    Animator animator;
    private PlayerControls playerControls;

    int moveXAnimationParamenterID;
    int moveZAnimationParamenterID;
    int magnitudeAnimationParamenterID;
    int isSprintingAnimationParamenterID;
    int isMovingnimationParamenterID;

    [SerializeField] bool isMoving = false;
    [SerializeField] bool isSprinting = false;
    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    InputAction.CallbackContext context;
    Vector2 input;

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
    }
    private void Update()
    {
        Animate();
    }


    void Animate()
    {

        // Check current inputs 
        input = context.ReadValue<Vector2>();
        isMoving = input.x != 0 || input.y != 0;

        if (isMoving) currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);
        else currentAnimationBlendVector = input;
        //Apply inputs in animator 
        animator.SetFloat(moveXAnimationParamenterID, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParamenterID, currentAnimationBlendVector.y);
        animator.SetBool(isMovingnimationParamenterID, isMoving);
        if (!isSprinting) animator.SetFloat(magnitudeAnimationParamenterID, currentAnimationBlendVector.magnitude);

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
        float num = context.ReadValue<float>();
        if (num == 1)
        {
            Debug.Log("MantainJump");
        }
        else if (num == 0)
        {
            Debug.Log("Tap");
        }

    }





}
