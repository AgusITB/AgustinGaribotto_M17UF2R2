using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    const float  cooldown = 1.1f;
    float cd = cooldown;
    bool playerCanShoot = true;
    Animator animator;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private GameObject cam;

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
            GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, Mathf.Infinity))
            {         
                bulletController.Target = (hit.point - transform.position).normalized;
                bulletController.Hit = true;
            }
            else
            {
                bulletController.Target = cam.transform.forward;// * 25f;
                bulletController.Hit = false;

            }
            playerCanShoot = false;
            animator.SetTrigger("playerShot");
        }
    }

}