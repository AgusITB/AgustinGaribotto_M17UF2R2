using System;
using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    public static Action saveTheGame;
    public void Interact()
    {
        saveTheGame.Invoke();
    }
}
