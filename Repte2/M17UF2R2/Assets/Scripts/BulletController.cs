using System.Runtime.CompilerServices;
using UnityEngine;
public class BulletController : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletDecal;

    private float speed = 50f;
    private float timeToDestroy = 3f;

    public Vector3 Target { get; set; }
    public bool Hit { get; set; }
    Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        Destroy(gameObject, timeToDestroy);
    }
    private void Update()
    {
        rb.AddForce(Target * speed, ForceMode.Impulse);
        //transform.position = Vector3.MoveTowards(transform.position, Target, speed,);
        if (!Hit && Vector3.Distance(transform.position, Target) < .01)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        ContactPoint contact= other.GetContact(0);
        Instantiate(bulletDecal, contact.point + contact.normal * .0001f, Quaternion.LookRotation(contact.normal));
        Destroy(gameObject);
    }
}
