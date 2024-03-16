using System.Collections.Generic;
using UnityEngine;

public class PlayerInvetory : MonoBehaviour
{
    private List<Item> itemsCollected = new List<Item>();

    public void AddToInventory(Item item)
    {
        Debug.Log(item);
        itemsCollected.Add(item);
        Debug.Log(item);
    }

    public void RemoveFromInventory(Item item)
    {
        itemsCollected.Remove(item);
    }



}
