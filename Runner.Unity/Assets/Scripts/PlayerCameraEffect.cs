using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;


public class PlayerCameraEffect : MonoBehaviour
{
    public Camera playerCamera;
    private Rigidbody rb;

    //Post Process
    public float playerHorizontalSpeed = 0f;
    public float baseFOV = 60f;
    public float nowFOV = 60f;
    public float maxFOV = 80f;
    public UniversalAdditionalCameraData UAC;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CameraEffect();
    }

    // Player Camera Effect (React by speed)
    private void CameraEffect()
    {
        playerHorizontalSpeed = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z).magnitude;
        if (playerHorizontalSpeed * 10 > baseFOV)
        {
            if (nowFOV <= maxFOV)
            {
                nowFOV += 0.1f;
            }
        }
        else
        {
            if(nowFOV > baseFOV)
            {
                nowFOV -= 0.5f;
            }
        }
        playerCamera.fieldOfView = nowFOV;

        if (nowFOV >= 60f)
            UAC.renderPostProcessing = true;
        else
            UAC.renderPostProcessing = false;
    }
}
