using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem.EnhancedTouch;

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
    int isCrouchingParameterID;
    

    // State Variables
    [SerializeField] bool isMoving = false;
    [SerializeField] bool isSprinting = false;
    [SerializeField] bool isJumping = false;
    [SerializeField] private bool groundedPlayer;
    [SerializeField] bool isAiming = false;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    // Movement variables
    private CharacterController controller;

    private Vector3 playerVelocity;
    private Transform cameraTransform;


    //Player variables
    [SerializeField] private float playerSpeed;
    [SerializeField] private float magnitude = 1f;
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 15f;
    

    //Input variables
    private InputManager inputManager;
    private Vector2 input;

    // Death delegates
    public delegate void OnPlayerDied();
    public static event OnPlayerDied PlayerDied;

    // Singleton
    private static PlayerController _instance;
    public static PlayerController Instance
    {
        get { return _instance; }
    }

    [SerializeField]
    private Transform aimTarget;
    [SerializeField]
    private float aimDistance = 40f;

    private Rig pistolArmRig;
    private bool isCrouching = false;
    private void Start()
    {
        inputManager = InputManager.Instance;
    }
    private void Awake()
    {
        //Singleton
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;

        SetComponents();
        SetAnimatorIDs();
    }
    private void SetComponents()
    {
        pistolArmRig = GetComponentInChildren<Rig>();
        controller = gameObject.GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        cameraTransform = Camera.main.transform;
    }
    private void SetAnimatorIDs()
    {
        moveXAnimationParamenterID = Animator.StringToHash("Velocity X");
        moveZAnimationParamenterID = Animator.StringToHash("Velocity Z");
        magnitudeAnimationParamenterID = Animator.StringToHash("Magnitude");
        isSprintingAnimationParamenterID = Animator.StringToHash("isSprinting");
        isMovingnimationParamenterID = Animator.StringToHash("isMoving");
        isJumpingParamenterID = Animator.StringToHash("isJumping");
        isGroundedParamenterID = Animator.StringToHash("isGrounded");
        isCrouchingParameterID = Animator.StringToHash("isCrouching");
    }

    private void OnEnable()
    {
        InputManager.PlayerJumped += Jump;
        InputManager.PlayerAimed += StartAiming;
        InputManager.PlayerDied += Die;
        InputManager.PlayerCrouched += Crouch;
    }
    private void OnDisable()
    {
        InputManager.PlayerJumped -= Jump;
        InputManager.PlayerAimed -= StartAiming;
        InputManager.PlayerDied -= Die;
        InputManager.PlayerCrouched -= Crouch;
    }
    private void Update()
    {
        // Check current inputs 

        aimTarget.position = cameraTransform.position + cameraTransform.forward * aimDistance;
        input = inputManager.GetPlayerMovement();
        CheckIfIsMoving();
        groundedPlayer = controller.isGrounded;
        magnitude = inputManager.PlayerStartedSprinting() + 1f;
        isSprinting = isMoving && magnitude > 1f;

        Move();
        Animate();
    }
    /// <summary>
    /// If the player releases a movement key we don't want to go instantly to Idle so we wait a little time to check 
    /// if the player pressed a key in that time so it doesn't show idle anim.
    /// </summary>
    private void CheckIfIsMoving()
    {
        if (!(input.x != 0 || input.y != 0)) StartCoroutine(WaitForBoolToChange());
        else isMoving = true;
    }
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

        playerSpeed = isSprinting ? sprintSpeed : walkSpeed;

        if (controller.enabled == true)
        {
            controller.Move(playerSpeed * Time.deltaTime * move);
        }
    }
    void Crouch()
    {
        isCrouching = !isCrouching;

        float weight = isCrouching ? 1f : 0f;
        animator.SetBool(isCrouchingParameterID, isCrouching);
        animator.SetLayerWeight(0, 0);
        StartCoroutine(SetWeightsTO(weight));


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
    private void StartAiming()
    {
        isAiming = !isAiming;
        int weight = isAiming ? 1 : 0;
        StartCoroutine(AnimateWeightsTo(weight));
    }

    private IEnumerator AnimateWeightsTo(float goalValue)
    {
        float startValue = pistolArmRig.weight;
        for (float i = 0f; i <= 1f; i += 5f * Time.deltaTime)
        {
            pistolArmRig.weight = Mathf.Lerp(startValue, goalValue, i);
            yield return null;
        }

        pistolArmRig.weight = goalValue;
    }
    private IEnumerator SetWeightsTO(float goalValue)
    {
        
        float startValue = animator.GetLayerWeight(1);
           
        for (float i = 0f; i <= 1f; i += 5f * Time.deltaTime)
        {
            animator.SetLayerWeight(1, Mathf.Lerp(startValue, goalValue, i));
            yield return null;
        }

       animator.SetLayerWeight(1, goalValue);
    }
    private void Die()
    {
        animator.Play("Death");
        PlayerDied.Invoke();
    }
}






