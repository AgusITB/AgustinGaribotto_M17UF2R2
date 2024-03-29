using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gun;
    [SerializeField]
    private GameObject crosshair;
    private bool gunIsActive = false;

    private void Awake()
    {
        Cursor.visible = false;
        gun.SetActive(false);
        crosshair.SetActive(false);
    }
    private void OnEnable()
    {
        InputManager.PlayerAimed += ActivateWeapon;

        PlayerController.PlayerDied += DisablePlayer;
    }

    
    private void OnDisable()
    {
        InputManager.PlayerAimed -= ActivateWeapon;
        PlayerController.PlayerDied -= DisablePlayer;
    }


    void DisablePlayer()
    {
        PlayerController.Instance.enabled = false;
        InputManager.Instance.gameObject.SetActive(false);
    }
    void ActivateWeapon()
    {
        gunIsActive = !gunIsActive;
        gun.SetActive(gunIsActive);
        crosshair.SetActive(gunIsActive);
    }
}
