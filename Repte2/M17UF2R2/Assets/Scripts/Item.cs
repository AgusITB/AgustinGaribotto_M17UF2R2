using System;
using UnityEngine;

[Serializable]
public abstract class Item : MonoBehaviour, ICollectable
{
    [SerializeField] private string Name;

    public abstract void Collect();
}

