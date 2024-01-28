using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.InputSystem.Utilities;
using System.Diagnostics;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    private PlayerControls playerControls;

    public UnityEvent playerJumped;

    public static InputManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
        playerControls = new PlayerControls();

        playerControls.Player.Jump.performed += context => playerJumped.Invoke();

    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
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
