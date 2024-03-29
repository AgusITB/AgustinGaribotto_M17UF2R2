using System;
using UnityEngine;


[Serializable]
public abstract class Item : MonoBehaviour, ICollectable, IInteractable
{
    public int id;
    public string Name { get; set; }

    public abstract void Collect();

    public void Interact()
    {
        Collect();
    }
}
