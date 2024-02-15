
using UnityEngine;

public class Weapon : MonoBehaviour
{

    const float  cooldown = 1.1f;
    float cd = cooldown;
    bool playerCanShoot = true;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {

        InputManager.Shoot += Shoot;
    }
    private void Update()
    {
        if (!playerCanShoot)
        {
            cd -= Time.deltaTime;

            if (cd < 0)
            {
                playerCanShoot = true;
                cd = cooldown;
            }
        }
    }
    private void Shoot()
    {

        if (PlayerController.IsAiming && playerCanShoot)
        {
            playerCanShoot = false;
            animator.SetTrigger("playerShot");
        }

    }

}