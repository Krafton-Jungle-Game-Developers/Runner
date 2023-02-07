using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerGame : MonoBehaviour
{

    //Camera Effect
    private GameObject _playerfollowcamera;
    private Camera _camera;

    private float PlayerSpped = 10f;

    private float BaseFOV = 40f;
    private float NowFOV = 40f;
    private float TargetFOV = 40f;

    // Start is called before the first frame update
    void Start()
    {
        //EDIT: Get Object/Component
        _playerfollowcamera = GameObject.FindGameObjectWithTag("CameraEffect");
        _camera = _playerfollowcamera.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        MotionBlur();
    }
    void MotionBlur()
    {
        
    }
}
