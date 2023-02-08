using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PlayerCameraEffect : MonoBehaviour
{
    public Camera playerCamera;
    private Rigidbody rb;

    //Reactive FOV
    public float playerHorizontalSpeed = 0f;
    public float oldPlayerHorizontalSpeed = 0f;
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
        oldPlayerHorizontalSpeed = playerHorizontalSpeed;
        CameraEffect();
    }

    // Player Camera Effect (React by speed)
/*    private float diffFOV = 0;
*/    private void CameraEffect()
    {
        playerHorizontalSpeed = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z).magnitude;
        if (playerHorizontalSpeed * 15 > baseFOV && (playerHorizontalSpeed - oldPlayerHorizontalSpeed) >= 0 )
        {
            //if (nowFOV <= maxFOV)
            //{
            //    nowFOV += 0.3f;
            //}
            //if (nowCAIntensity <= maxCAIntensity)
            //{
            //    nowCAIntensity += 0.03f;
            //}
            //if (nowMBIntensity <= maxMBIntensity)
            //{
            //    nowMBIntensity += 0.03f;
            //}
            //if (nowBloomIntensity <= maxBloomIntensity)
            //{
            //    nowBloomIntensity += 0.03f;
            //}
            nowFOV = Mathf.Lerp(nowFOV, maxFOV, Time.deltaTime * playerHorizontalSpeed);
            nowCAIntensity = Mathf.Lerp(nowCAIntensity, maxCAIntensity, Time.deltaTime * playerHorizontalSpeed);
            nowMBIntensity = Mathf.Lerp(nowMBIntensity, maxMBIntensity, Time.deltaTime * playerHorizontalSpeed);
            nowBloomIntensity = Mathf.Lerp(nowBloomIntensity, maxBloomIntensity, Time.deltaTime * playerHorizontalSpeed);
        }
        else
        //if ((playerHorizontalSpeed - oldPlayerHorizontalSpeed) < 0)
        {
            //if (nowFOV > baseFOV)
            //{
            //    nowFOV -= 0.7f;
            //}
            //if (nowCAIntensity > baseCAIntensity)
            //{
            //    nowCAIntensity -= 0.07f;
            //}
            //if (nowMBIntensity > baseMBIntensity)
            //{
            //    nowMBIntensity -= 0.07f;
            //}
            //if (nowBloomIntensity > baseBloomIntensity)
            //{
            //    nowBloomIntensity -= 0.07f;
            //}
            nowFOV = Mathf.Lerp(nowFOV, baseFOV, Time.deltaTime * playerHorizontalSpeed);
            nowCAIntensity = Mathf.Lerp(nowCAIntensity, baseCAIntensity, Time.deltaTime * playerHorizontalSpeed);
            nowMBIntensity = Mathf.Lerp(nowMBIntensity, baseMBIntensity, Time.deltaTime * playerHorizontalSpeed);
            nowBloomIntensity = Mathf.Lerp(nowBloomIntensity, baseBloomIntensity, Time.deltaTime * playerHorizontalSpeed);
        }
        playerCamera.fieldOfView = nowFOV;
        ChromaticAberration.intensity.value = nowCAIntensity;
        MotionBlur.intensity.value = nowMBIntensity;
        Bloom.intensity.value = nowBloomIntensity;      
    }
}