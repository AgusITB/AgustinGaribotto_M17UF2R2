using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    // Animation variables
    [SerializeField] float animationSmoothTime = 1f;
    Animator animator;

    int moveXAnimationParameterID, moveZAnimationParameterID, magnitudeAnimationParameterID,
        isSprintingAnimationParameterID, isMovingnimationParameterID, isJumpingParameterID,
        isGroundedParameterID, isCrouchingParameterID, isFallingParameterID;


    // State Variables
    [Header("PlayerStates")]
    [SerializeField] bool isMoving;
    [SerializeField] private static bool isAiming;
    public static bool IsAiming { get { return isAiming; } }
    [SerializeField] bool IsSprinting { get; set; }
    [SerializeField] bool IsJumping { get; set; }
    [SerializeField] bool GroundedPlayer { get; set; }
    [SerializeField] bool IsCrouching { get; set; }
    [SerializeField] bool IsFalling { get; set; }

    //[SerializeField] bool isLanding = false;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    // Movement variables
    private CharacterController controller;

    private Vector3 playerVelocity;
    private Transform cameraTransform;


    //Player variables
    [Header("PlayerVariables")]
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
        moveXAnimationParameterID = Animator.StringToHash("Velocity X");
        moveZAnimationParameterID = Animator.StringToHash("Velocity Z");
        magnitudeAnimationParameterID = Animator.StringToHash("Magnitude");
        isSprintingAnimationParameterID = Animator.StringToHash("isSprinting");
        isMovingnimationParameterID = Animator.StringToHash("isMoving");
        isJumpingParameterID = Animator.StringToHash("isJumping");
        isGroundedParameterID = Animator.StringToHash("isGrounded");
        isCrouchingParameterID = Animator.StringToHash("isCrouching");
        isFallingParameterID = Animator.StringToHash("isFalling");
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
        GroundedPlayer = controller.isGrounded;
        magnitude = inputManager.PlayerStartedSprinting() + 1f;
        IsSprinting = isMoving && magnitude > 1f;

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
        if (GroundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            IsJumping = false;
            IsFalling = false;
        }

        if (controller.velocity.y < 0f && !GroundedPlayer) IsFalling = true;

        Vector3 move = new(input.x, 0, input.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;

        move.y = playerVelocity.y += gravityValue * Time.deltaTime;

        if (!IsCrouching) playerSpeed = IsSprinting ? sprintSpeed : walkSpeed;
        else if (IsCrouching) playerSpeed = IsSprinting ? 1.5f : 1;

        if (controller.enabled == true) controller.Move(playerSpeed * Time.deltaTime * move);
    }
    private void Crouch()
    {
        if (IsCrouching && Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.up), Mathf.Infinity, LayerMask.GetMask("Environment"))) return;

        IsCrouching = !IsCrouching;

        float weight = IsCrouching ? 1f : 0f;
        animator.SetBool(isCrouchingParameterID, IsCrouching);
        animator.SetLayerWeight(0, 0);
        StartCoroutine(SetWeightsTO(weight));

        if (IsCrouching)
        {
            controller.height = 0.8f;
            controller.center = new(0f, 0.5f, 0f);
        }
        else
        {
            controller.height = 1.61f;
            controller.center = new(0f, 0.89f, 0f);
        }
    }
    public void Jump()
    {
        if (GroundedPlayer && controller.enabled == true && !IsCrouching)
        {
            IsJumping = true;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3f * gravityValue);
        }
    }

    void Animate()
    {
        if (isMoving) currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);
        else currentAnimationBlendVector = input;

        ApplySettingsToAnimator();

        if (!IsSprinting) animator.SetFloat(magnitudeAnimationParameterID, currentAnimationBlendVector.magnitude);

        //Rotate towards camera direction
        Quaternion rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

    }

    void ApplySettingsToAnimator()
    {
        //Apply inputs in animator 
        animator.SetFloat(moveXAnimationParameterID, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParameterID, currentAnimationBlendVector.y);
        animator.SetBool(isMovingnimationParameterID, isMoving);
        animator.SetBool(isGroundedParameterID, GroundedPlayer);
        animator.SetBool(isJumpingParameterID, IsJumping);
        animator.SetFloat(magnitudeAnimationParameterID, magnitude);
        animator.SetBool(isSprintingAnimationParameterID, IsSprinting);
        animator.SetBool(isFallingParameterID, IsFalling);
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