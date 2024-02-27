using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField] PlayerController controller;


    private void Start()
    {
        //controller.transform.position = new Vector3(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"), PlayerPrefs.GetFloat("z")); 
        
    }
    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetFloat("x", controller.transform.position.x);
        PlayerPrefs.SetFloat("y", controller.transform.position.y);
        PlayerPrefs.SetFloat("z", controller.transform.position.z);
    }

    void SavePositionSettings()
    {
        PlayerPrefs.SetFloat("x", controller.transform.position.x);
        PlayerPrefs.SetFloat("y", controller.transform.position.y);
        PlayerPrefs.SetFloat("z", controller.transform.position.z);
    }
}
