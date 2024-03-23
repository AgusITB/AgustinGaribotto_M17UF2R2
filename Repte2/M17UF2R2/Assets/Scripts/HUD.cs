using System;
using TMPro;
using UnityEngine;



public class HUD : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [SerializeField] private GameObject messagePanel;

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

    public void OpenItemPanel(IInteractable interactable)
    {
        messagePanel.SetActive(true);
        string text = "";
        if (interactable is Item)
        {
            Item item = interactable as Item;   
            text = "Press E to take " + item.name;
        }
        else if (interactable is SavePoint)
        {
            SavePoint sP = interactable as SavePoint;
            text = "Press E to save the game";
        }
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = text;

    }
    public void CloseItemPanel()
    {
        messagePanel.SetActive(false);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }
}
