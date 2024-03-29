using UnityEngine;
using Cinemachine;

public class SwapCam : MonoBehaviour
{
    [SerializeField] int priorityBoostAmount = 10;
    private CinemachineVirtualCamera virtualCamera;
    private bool isPriorityBoosted = false;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        virtualCamera.enabled = false;
    }
    private void OnEnable()
    {
        InputManager.PlayerAimed += StartAim;
    }
    private void OnDisable()
    {
        InputManager.PlayerAimed -= StartAim;    
    }
    // When the player aims we swap the camera from third person to first person changing the priority of the 1st person camera.
    private void StartAim()
    {
        virtualCamera.enabled = !isPriorityBoosted;
        isPriorityBoosted = !isPriorityBoosted;

        virtualCamera.Priority += isPriorityBoosted ? +priorityBoostAmount : -priorityBoostAmount;
    }

}
