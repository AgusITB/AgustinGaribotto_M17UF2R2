using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;

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
    [SerializeField] bool isAiming = false;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    // Movement variables
    Rigidbody rb;
    private float lastYposition;
 


    private Transform cameraTransform;


    //Player variables
    [SerializeField] private float playerSpeed;
    [SerializeField] private float magnitude = 1f;
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float maxForce = 1f;

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
        lastYposition = transform.position.y;
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
        animator = GetComponentInChildren<Animator>();
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();   
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
    }

    private void OnEnable()
    {
        InputManager.PlayerJumped += Jump;
        InputManager.PlayerAimed += StartAiming;
        InputManager.PlayerDied += Die;
    }
    private void OnDisable()
    {
        InputManager.PlayerJumped -= Jump;
        InputManager.PlayerAimed -= StartAiming;
        InputManager.PlayerDied -= Die;
    }

    private void FixedUpdate()
    {
     
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity *= playerSpeed;

        //Align direction
        targetVelocity = transform.TransformDirection(targetVelocity);

        //Calculate forces
        Vector3 velocityChange = (targetVelocity - currentVelocity);

        //Limit force
        Vector3.ClampMagnitude(velocityChange, maxForce);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

    }
    private void Update()
    {
        // Check current inputs 

        aimTarget.position = cameraTransform.position + cameraTransform.forward * aimDistance;
        isMoving = rb.velocity.magnitude > 0.2f;
        groundedPlayer = (lastYposition == transform.position.y); // Checks if Y has changed since last frame
        lastYposition = transform.position.y;
        input = inputManager.GetPlayerMovement();     
        magnitude = inputManager.PlayerStartedSprinting() + 1f;
        isSprinting = isMoving && magnitude > 1f;
    
        Animate();
    }
    /// <summary>
    /// If the player releases a movement key we don't want to go instantly to Idle so we wait a little time to check 
    /// if the player pressed a key in that time so it doesn't show idle anim.
    /// </summary>
    private void Jump()
    {

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
    private void Die()
    {
        animator.Play("Death");
        PlayerDied.Invoke();
    }
}






