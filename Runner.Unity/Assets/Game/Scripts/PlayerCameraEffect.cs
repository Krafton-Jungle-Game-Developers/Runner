using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PlayerCameraEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private UniversalAdditionalCameraData UAC;
    [Space]

    //Reactive FOV
    [SerializeField] private float playerVelocity = 0f;
    [SerializeField] private float oldPlayerVelocity = 0f;
    [SerializeField] private float playerAcceleration = 0f;

    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private float nowFOV = 60f;
    [SerializeField] private float maxFOV = 80f;
    [Space]

    [SerializeField] private float baseCAIntensity = 0f;
    [SerializeField] private float nowCAIntensity = 0f;
    [SerializeField] private float maxCAIntensity = 0.8f;
    [Space]

    [SerializeField] private float baseMBIntensity = 0f;
    [SerializeField] private float nowMBIntensity = 0f;
    [SerializeField] private float maxMBIntensity = 0.5f;
    [Space]

    [SerializeField] private float baseBloomIntensity = 0f;
    [SerializeField] private float nowBloomIntensity = 0f;
    [SerializeField] private float maxBloomIntensity = 0.5f;

    private Bloom _bloom;
    private ChromaticAberration _chromaticAberration;
    private MotionBlur _motionBlur;

    private void Awake()
    {
        UAC = playerCamera.GetComponent<UniversalAdditionalCameraData>();

        globalVolume.profile.TryGet(out _motionBlur);
        globalVolume.profile.TryGet(out _chromaticAberration);
        globalVolume.profile.TryGet(out _bloom);

    }

    private void Update()
    {
        CameraEffect();
    }

    private void FixedUpdate()
    {
        //Get player's Velocity & cceleration
        playerVelocity = new Vector3(playerRigidbody.velocity.x,
                                     0.0f,
                                     playerRigidbody.velocity.z).magnitude;

        //TODO: don't use Deltatime
        playerAcceleration = (playerVelocity - oldPlayerVelocity) / Time.deltaTime;
        oldPlayerVelocity = new Vector3(playerRigidbody.velocity.x,
                                        0.0f,
                                        playerRigidbody.velocity.z).magnitude;
    }

    private void CameraEffect()
    {
        if (playerVelocity * 15 > baseFOV && playerAcceleration >= 0 )
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
            //Not use Delta Time
            nowFOV = Mathf.Lerp(nowFOV, maxFOV,                                 0.001f * playerVelocity);
            nowCAIntensity = Mathf.Lerp(nowCAIntensity, maxCAIntensity,         0.001f * playerVelocity);
            nowMBIntensity = Mathf.Lerp(nowMBIntensity, maxMBIntensity,         0.001f * playerVelocity);
            nowBloomIntensity = Mathf.Lerp(nowBloomIntensity, maxBloomIntensity,0.001f * playerVelocity);
        }
        else if(playerVelocity * 15 <= baseFOV)
        //if (playerHorizontalSpeed * 15 <= baseFOV  (playerHorizontalSpeed - oldPlayerHorizontalSpeed) < 0)
        {
            if (nowFOV > baseFOV)
            {
                nowFOV -= 0.7f;
            }
            if (nowCAIntensity > baseCAIntensity)
            {
                nowCAIntensity -= 0.07f;
            }
            if (nowMBIntensity > baseMBIntensity)
            {
                nowMBIntensity -= 0.07f;
            }
            if (nowBloomIntensity > baseBloomIntensity)
            {
                nowBloomIntensity -= 0.07f;
            }

            //NOTE: Not use deltatime
            //nowFOV = Mathf.Lerp(nowFOV, baseFOV, Time.deltaTime * playerHorizontalSpeed);
            //nowCAIntensity = Mathf.Lerp(nowCAIntensity, baseCAIntensity, Time.deltaTime * playerHorizontalSpeed);
            //nowMBIntensity = Mathf.Lerp(nowMBIntensity, baseMBIntensity, Time.deltaTime * playerHorizontalSpeed);
            //nowBloomIntensity = Mathf.Lerp(nowBloomIntensity, baseBloomIntensity, Time.deltaTime * playerHorizontalSpeed
        }
        playerCamera.fieldOfView = nowFOV;

        _chromaticAberration.intensity.value = nowCAIntensity;
        _motionBlur.intensity.value = nowMBIntensity;
        _bloom.intensity.value = nowBloomIntensity;    
    }
}