using System;
using System.Collections;
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
       
   
    }
    private void Start()
    {
        inventory = player.GetComponent<PlayerInvetory>();
        StartCoroutine(LoadData());
     
    }
    private IEnumerator LoadData()
    {
        yield return new WaitForSeconds(1f);
        if (File.Exists(saveFile))
        {           
            string content = File.ReadAllText(saveFile);
            if (content  == "") { yield break; }
            playerSettings = JsonUtility.FromJson<PlayerData>(content);            

            dataLoaded.Invoke(playerSettings);
            Debug.Log("Player position loaded: " + playerSettings.playerPosition);
            Debug.Log(saveFile);
            player.transform.position = playerSettings.playerPosition;

            foreach (Item item in playerSettings.itemsCollected)
            {
                item.Collect();
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
        string json;

        PlayerData newPlayerSettings = new()
        {
            playerPosition = player.transform.position,
            itemsCollected = inventory.itemObjects
        };


        json = JsonUtility.ToJson(newPlayerSettings);    
        File.WriteAllText(saveFile, json);

        Debug.Log("Saved data");
    }
}

