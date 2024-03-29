using UnityEngine;

public class Inventory : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void ToggleInventory()
    {
        if (!gameObject.activeInHierarchy) 
            gameObject.SetActive(true);
        else 
            gameObject.SetActive(false);

    }
}
