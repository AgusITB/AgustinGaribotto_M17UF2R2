using System.Collections;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [SerializeField] private GameObject messagePanel;
    [SerializeField] private GameObject gameStateMessagePanel;

    private void OnEnable()
    {
        InputManager.OpenInventory += ToggleInventory;
        GameDataManager.gameSaved += ShowStateMessage;
    }
    private void OnDisable()
    {
        InputManager.OpenInventory -= ToggleInventory;
        GameDataManager.gameSaved -= ShowStateMessage;
    }
    private void ToggleInventory()
    {
        inventory.ToggleInventory();
    }

    private void ShowStateMessage()
    {
        StartCoroutine(GameStateMessage());
    }
    private IEnumerator GameStateMessage()
    {
        gameStateMessagePanel.SetActive(true);
        yield return new WaitForSeconds(5f);
        gameStateMessagePanel.SetActive(false);
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
