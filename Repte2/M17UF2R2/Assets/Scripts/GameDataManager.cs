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

    PlayerInvetory inventory;

    public static Action<PlayerData> dataLoaded;

    private void OnEnable()
    {
        SavePoint.saveTheGame += SaveData;
    }
    private void OnDisable()
    {
        SavePoint.saveTheGame -= SaveData;
    }


    private void Awake()
    {
        saveFile = Application.dataPath + "/savedData.json";
        Debug.Log(saveFile);
        
   
    }
    private void Start()
    {
        inventory = player.GetComponent<PlayerInvetory>();
        LoadData();
    }
    private void LoadData()
    {
        if (File.Exists(saveFile))
        {
            
            string content = File.ReadAllText(saveFile);
            if (content  == "") { return; }
            playerSettings = JsonUtility.FromJson<PlayerData>(content);            

            dataLoaded.Invoke(playerSettings);
            Debug.Log("Player position loaded: " + playerSettings.playerPosition);
            player.transform.position = playerSettings.playerPosition;

            foreach (ItemData item in playerSettings.itemsCollected)
            {
                inventory.AddToInventory(item);
            }
        
        }
        else
        {
            File.Create(saveFile);
            Debug.Log("Archivo creado");
        }
    }
    private void SaveData()
    {
        string json ="";
   
        PlayerData newPlayerSettings = new()
        {
            playerPosition = player.transform.position,
            itemsCollected = inventory.itemsCollected
        };


        json = JsonUtility.ToJson(newPlayerSettings);    
        File.WriteAllText(saveFile, json);

        Debug.Log("Saved data");
    }
}

