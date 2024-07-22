using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxControll : MonoBehaviour
{
    public float skyBoxRotationSpeed = 1.0f;
    
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyBoxRotationSpeed);
    }
}
