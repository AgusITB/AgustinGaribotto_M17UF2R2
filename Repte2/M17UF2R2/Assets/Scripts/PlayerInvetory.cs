using System.Collections.Generic;
using UnityEngine;

public class PlayerInvetory : MonoBehaviour
{
    public List<Item> itemObjects = new();

    public void AddToList(Item item)
    {
        itemObjects.Add(item);
    }
}
