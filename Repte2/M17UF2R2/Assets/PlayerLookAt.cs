using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookAt : MonoBehaviour
{

    Animator animator;
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetLookAtWeight(0.5f, 0.1f, 0.5f, 0.5f, .5f);

        Vector3 directionToLookAT = mainCamera.transform.eulerAngles;
        directionToLookAT.y = 0f;   
        Ray lookAtRay = new Ray(transform.position, mainCamera.transform.eulerAngles);
        animator.SetLookAtPosition(lookAtRay.GetPoint(5f));
    }
}
