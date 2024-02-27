using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayerMap : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void Update()
    {
        this.transform.position = new Vector3(target.transform.position.x,this.transform.position.y, target.transform.position.z);
    }
}
