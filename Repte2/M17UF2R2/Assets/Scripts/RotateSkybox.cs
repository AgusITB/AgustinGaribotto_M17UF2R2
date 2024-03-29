using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
    }
}

