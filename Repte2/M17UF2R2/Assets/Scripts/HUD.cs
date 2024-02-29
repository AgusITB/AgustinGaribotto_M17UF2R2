using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    private void OnEnable()
    {
        InputManager.OpenInventory += ToggleInventory;
    }
    private void OnDisable()
    {
        InputManager.OpenInventory -= ToggleInventory;
    }
    private void ToggleInventory()
    {
        inventory.ToggleInventory();
    }
}
