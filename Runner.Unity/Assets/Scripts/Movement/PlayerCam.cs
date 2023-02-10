using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float xSensitivity;
    public float ySensitivity;
    public Transform orientation;

    private float _xRotation;
    private float _yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //FIXED: orientation before 
        //float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSensitivity;
        //float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySensitivity;

        float mouseX = Input.GetAxisRaw("Mouse X") * 0.001f * xSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * 0.001f * ySensitivity;

        _yRotation += mouseX;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);

    }
}
