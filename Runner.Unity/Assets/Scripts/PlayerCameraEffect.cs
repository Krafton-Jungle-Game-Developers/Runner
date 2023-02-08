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
    public float MaxFOV = 110f;
    public float baseFOV = 60f;
    public float nowFOV = 60f;
    public float maxFOV = 80;
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
        if (playerHorizontalSpeed * 10 > baseFOV && playerHorizontalSpeed * 10 < maxFOV)
        {
            nowFOV = (playerHorizontalSpeed * 10 + baseFOV) / 2;
        }
        else if (playerHorizontalSpeed * 10 < baseFOV)
        {
            nowFOV = baseFOV;

        }
        else
        {
            nowFOV = maxFOV;
        }
        playerCamera.fieldOfView = nowFOV;

        if (nowFOV >= 60f)
            UAC.renderPostProcessing = true;
        else
            UAC.renderPostProcessing = false;
    }
}
