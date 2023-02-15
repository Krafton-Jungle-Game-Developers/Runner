using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private float xSensitivity = 500f;
    [SerializeField] private float ySensitivity = 500f;
    public Transform orientation;
    public bool freezeMouse;

    public float _xRotation;
    public float _yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //FIXED: orientation before 
        //float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSensitivity;
        //float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySensitivity;
        if (!freezeMouse)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * 0.01f * xSensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * 0.01f * ySensitivity;

            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        }
        if (freezeMouse)
        {
            _xRotation = transform.rotation.eulerAngles.x;
            _yRotation = transform.rotation.eulerAngles.y;
            if (_xRotation > 90)
            {
                _xRotation -= 360;
            }
        }
    }
}