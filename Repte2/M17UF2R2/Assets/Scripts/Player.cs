using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerControls playerControls;
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
        playerControls = new PlayerControls();
        playerControls.Player.Jump.performed += Jump;

    }

    void Jump(InputAction.CallbackContext context)
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
