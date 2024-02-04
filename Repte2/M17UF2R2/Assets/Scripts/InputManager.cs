using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    private PlayerControls playerControls;


    // Jump delegates
    public delegate void OnPlayerJumped();
    public static event OnPlayerJumped PlayerJumped;

    // Aim delegates
    public delegate void OnPlayerAimed();
    public static event OnPlayerAimed PlayerAimed;


    // Death delegates
    public delegate void OnPlayerDied();
    public static event OnPlayerAimed PlayerDied;

    public static InputManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
        playerControls = new PlayerControls();
    
    }
    private void OnEnable()
    {
        playerControls.Player.Jump.started += _ => PlayerJumped.Invoke();
        playerControls.Player.Aim.performed += _ => PlayerAimed.Invoke();
        playerControls.Player.DieTestButton.performed += _ => PlayerDied.Invoke();
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Player.Jump.started -= _ => PlayerJumped.Invoke();
        playerControls.Player.Aim.performed -= _ => PlayerAimed.Invoke();
        playerControls.Player.DieTestButton.performed -= _ => PlayerDied.Invoke();
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()  
    { 
        return playerControls.Player.Move.ReadValue<Vector2>();
    }
    public float PlayerStartedSprinting()
    {
        return playerControls.Player.Sprint.ReadValue<float>();
    }

}
