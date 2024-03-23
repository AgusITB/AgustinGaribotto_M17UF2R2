using System;
using UnityEngine;

[Serializable]
public abstract class Item : MonoBehaviour, ICollectable, IInteractable
{
    public string Name { get; set; }

    public abstract void Collect();

    public void Interact()
    {
        Collect();
    }
}

[Serializable]
public class ItemData
{
    public string name;
    public Item item;


    public ItemData(string name, Item item)
    {
        this.name = name;
        this.item = item;
    }
    public ItemData(Item item)
    {
        this.name = item.Name;
        this.item = item;
    }
}
