using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;


public class PlayerCameraEffect : MonoBehaviour
{
    private Rigidbody rb;

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    //Post Process
    public bool enableZoom = true;
    public bool holdToZoom = false;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    public float PlayerHorizontalSpeed = 0f;
    public float MaxFOV = 110f;
    public UniversalAdditionalCameraData UAC;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Start is called before the first frame update
    void Start()
    {
        crosshairObject = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraEffect();
    }

    // Player Camera Effect (React by speed)
    private void CameraEffect()
    {
        if (cameraCanMove)
        {
            yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

            if (!invertCamera)
            {
                pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            }
            else
            {
                // Inverted Y
                pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
            }

            // Clamp pitch between lookAngle
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        //Reactive FOV with Player's Velocity
        PlayerHorizontalSpeed = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z).magnitude;
        playerCamera.fieldOfView = PlayerHorizontalSpeed * 10 < fov ? fov : PlayerHorizontalSpeed * 10 > MaxFOV ? MaxFOV : PlayerHorizontalSpeed * 10;

        if (PlayerHorizontalSpeed > 6f)
            UAC.renderPostProcessing = true;
        else
            UAC.renderPostProcessing = false;


    }
}
