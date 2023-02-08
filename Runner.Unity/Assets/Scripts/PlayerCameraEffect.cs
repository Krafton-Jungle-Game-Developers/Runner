using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;



public class PlayerCameraEffect : MonoBehaviour
{
    public Camera playerCamera;
    private Rigidbody rb;

    //Reactive FOV
    public float playerHorizontalSpeed = 0f;
    public float baseFOV = 60f;
    public float nowFOV = 60f;
    public float maxFOV = 80f;
    public UniversalAdditionalCameraData UAC;

    //Post processing
    public Volume Volume;
    public MotionBlur MotionBlur;
    public ChromaticAberration ChromaticAberration;
    public Bloom Bloom;

    public float baseCAIntensity = 0f;
    public float nowCAIntensity = 0f;
    public float maxCAIntensity = 0.8f;

    public float baseMBIntensity = 0f;
    public float nowMBIntensity = 0f;
    public float maxMBIntensity = 0.5f;

    public float baseBloomIntensity = 0f;
    public float nowBloomIntensity = 0f;
    public float maxBloomIntensity = 0.5f;



    void Awake()
    {
        UAC = playerCamera.GetComponent<UniversalAdditionalCameraData>();
        rb = GetComponent<Rigidbody>();
        Volume.profile.TryGet(out MotionBlur);
        Volume.profile.TryGet(out ChromaticAberration);
        Volume.profile.TryGet(out Bloom);

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
            if (nowCAIntensity <= maxCAIntensity)
            {
                nowCAIntensity += 0.01f;
            }
            if (nowMBIntensity <= maxMBIntensity)
            {
                nowMBIntensity += 0.01f;
            }
            if (nowBloomIntensity <= maxBloomIntensity)
            {
                nowBloomIntensity += 0.01f;
            }
        }
        else
        {
            if (nowFOV > baseFOV)
            {
                nowFOV -= 0.5f;
            }
            if (nowCAIntensity > baseCAIntensity)
            {
                nowCAIntensity -= 0.05f;
            }
            if (nowMBIntensity > baseMBIntensity)
            {
                nowMBIntensity -= 0.05f;
            }
            if (nowBloomIntensity > baseBloomIntensity)
            {
                nowBloomIntensity -= 0.05f;
            }
        }
        playerCamera.fieldOfView = nowFOV;
        ChromaticAberration.intensity.value = nowCAIntensity;
        MotionBlur.intensity.value = nowMBIntensity;
        Bloom.intensity.value = nowBloomIntensity;
    }
}