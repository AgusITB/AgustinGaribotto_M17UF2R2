using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    private PlayerControls playerControls;


    // Jump delegates
    public static Action PlayerJumped;

    // Aim delegates
    public static Action PlayerAimed;

    // Crouch delegates
    public static Action PlayerCrouched;

    // Death delegates
    public static Action PlayerDied;

    // Shoot delegates
    public static Action Shoot;

    // Inventory delegate
    public static Action OpenInventory;

    public static Action PickupItem;

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
        playerControls.Player.Crouch.performed += _ => PlayerCrouched.Invoke();
        playerControls.Player.Shoot.performed += _ => Shoot.Invoke();
        playerControls.Player.OpenInventory.performed += _ => OpenInventory.Invoke();
        playerControls.Player.PickUpItem.performed += _ => PickupItem.Invoke();
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Player.Jump.started -= _ => PlayerJumped.Invoke();
        playerControls.Player.Aim.performed -= _ => PlayerAimed.Invoke();
        playerControls.Player.DieTestButton.performed -= _ => PlayerDied.Invoke();
        playerControls.Player.Crouch.performed -= _ => PlayerCrouched.Invoke();
        playerControls.Player.Shoot.performed -= _ => Shoot.Invoke();
        playerControls.Player.OpenInventory.performed -= _ => OpenInventory.Invoke();
        playerControls.Player.PickUpItem.performed -= _ => PickupItem.Invoke();
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