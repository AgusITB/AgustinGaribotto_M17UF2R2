using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    public string saveFile;

    public PlayerData playerSettings = new();


    public static Action<PlayerData> dataLoaded;

    private void OnEnable()
    {
        InputManager.PickupItem += SaveData;
    }
    private void OnDisable()
    {
        InputManager.PickupItem -= SaveData;
    }


    private void Awake()
    {
        saveFile = Application.dataPath + "/savedData.json";
   
    }
    private void Start()
    {
        LoadData();
    }
    private void LoadData()
    {
        if (File.Exists(saveFile))
        {
            string content = File.ReadAllText(saveFile);
            playerSettings = JsonUtility.FromJson<PlayerData>(content);
            dataLoaded.Invoke(playerSettings);
            Debug.Log("Player position saved: " + playerSettings.playerPosition);
        }
        else
        {
            Debug.Log("El archivo no existe");
        }
    }
    private void SaveData()
    {
        PlayerData newPlayerSettings = new()
        {
            playerPosition = player.transform.position,      
        };
        string json = JsonUtility.ToJson(newPlayerSettings);    
        File.WriteAllText(saveFile, json);

        Debug.Log("Saved data");
    }
}

