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

    public void OpenItemPanel(Item item)
    {
        messagePanel.SetActive(true);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text= "Press E to take " + item.name;
        
    }
    public void CloseItemPanel()
    {
        messagePanel.SetActive(false);
        messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }
}
