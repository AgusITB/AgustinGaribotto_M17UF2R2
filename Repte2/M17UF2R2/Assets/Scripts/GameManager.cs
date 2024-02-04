using UnityEngine;

public class GameManager : MonoBehaviour
{

    private void Awake()
    {
        Cursor.visible = false;
  
    }

    private void OnEnable()
    {
        PlayerController.PlayerDied += DisablePlayer;
    }

    
    private void OnDisable()
    {
        PlayerController.PlayerDied -= DisablePlayer;
    }


    void DisablePlayer()
    {
        PlayerController.Instance.enabled = false;
        InputManager.Instance.gameObject.SetActive(false);
    }

}
