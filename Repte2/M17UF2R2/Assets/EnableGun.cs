using UnityEngine;

public class EnableGun : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
