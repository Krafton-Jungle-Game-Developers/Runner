using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UIElements.Experimental;
using UnityEngine.Playables;
using SCPE;

public class PlayerCameraEffect : MonoBehaviour
{
    PlayerMovementController playerMovementController;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private UniversalAdditionalCameraData UAC;
    [Space]

    [SerializeField] private float lastingDuration = 1f;
    [Space]
    //Reactive FOV
    [SerializeField] private float playerVelocity = 0f;
    [SerializeField] private float oldPlayerVelocity = 0f;
    [SerializeField] private float playerAcceleration = 0f;
    [Space]

    [Header("FOV")]
    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private float nowFOV = 60f;
    [SerializeField] private float maxFOV = 80f;
    [Space]

    [Header("Chromatic Aberration")]
    [SerializeField] private float baseCAIntensity = 0f;
    [SerializeField] private float nowCAIntensity = 0f;
    [SerializeField] private float maxCAIntensity = 0.8f;
    [Space]

    [Header("Motion Blur")]
    [SerializeField] private float baseMBIntensity = 0f;
    [SerializeField] private float nowMBIntensity = 0f;
    [SerializeField] private float maxMBIntensity = 0.5f;
    [Space]

    [Header("Bloom")]
    [SerializeField] private float baseBloomIntensity = 0f;
    [SerializeField] private float nowBloomIntensity = 0f;
    [SerializeField] private float maxBloomIntensity = 0.5f;
    [Space]

    [Header("RadialBlur")]
    [SerializeField] private float baseRadialBlurIntensity = 0f;
    [SerializeField] private float nowRadialBlurIntensity = 0f;
    [SerializeField] private float maxRadialBlurIntensity = 0.5f;
    [Space]

    [Header("SpeedLine")]
    [SerializeField] private ParticleSystem _speedParticleSystem;
    private ParticleSystem.EmissionModule _speedParticleEmission;
    [Space]

    [SerializeField] private float baseParticleIntensity = 0f;
    [SerializeField] private float nowParticleIntensity = 0f;
    [SerializeField] private float maxParticleIntensity = 50f;
    [Space]

    private Bloom _bloom;
    private ChromaticAberration _chromaticAberration;
    private MotionBlur _motionBlur;
    private RadialBlur _radialBlur;


    private void Awake()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
        UAC = playerCamera.GetComponent<UniversalAdditionalCameraData>();

        globalVolume.profile.TryGet(out _motionBlur);
        globalVolume.profile.TryGet(out _chromaticAberration);
        globalVolume.profile.TryGet(out _bloom);
        globalVolume.profile.TryGet(out _radialBlur);
        _speedParticleEmission = _speedParticleSystem.emission;
    }

    private void Update()
    {
        FowordCameraEffect();
    }

    private void FixedUpdate()
    {
        //Get player's Velocity & cceleration
        playerVelocity = new Vector3(playerRigidbody.velocity.x,
                                     0.0f,
                                     playerRigidbody.velocity.z).magnitude;

        //NOTE: Don't use Delta Time (Jittering)
        playerAcceleration = (playerVelocity - oldPlayerVelocity) / Time.deltaTime;
        oldPlayerVelocity = new Vector3(playerRigidbody.velocity.x,
                                        0.0f,
                                        playerRigidbody.velocity.z).magnitude;
    }

    private void FowordCameraEffect()
    {
        if (playerMovementController.state == MovementState.Dashing)
        //if(playerVelocity * 15 > baseFOV && playerAcceleration > 0)
        {
            //NOTE: Don't use Delta Time (Jittering)
            nowFOV = Mathf.Lerp(nowFOV, maxFOV, 0.001f * playerVelocity);
            nowCAIntensity = Mathf.Lerp(nowCAIntensity, maxCAIntensity, 0.001f * playerVelocity);
            nowMBIntensity = Mathf.Lerp(nowMBIntensity, maxMBIntensity, 0.001f * playerVelocity);
            nowBloomIntensity = Mathf.Lerp(nowBloomIntensity, maxBloomIntensity, 0.001f * playerVelocity);
            nowRadialBlurIntensity = Mathf.Lerp(nowRadialBlurIntensity, maxRadialBlurIntensity, 0.001f * playerVelocity);
            nowParticleIntensity = Mathf.Lerp(nowParticleIntensity, maxParticleIntensity, 0.1f * playerVelocity);

        }
        else
        {
            //NOTE: Don't use Delta Time (Jittering)
            if (nowFOV > baseFOV)
            {
                nowFOV -= 0.7f * lastingDuration;
            }
            if (nowCAIntensity > baseCAIntensity)
            {
                nowCAIntensity -= 0.07f * lastingDuration;
            }
            if (nowMBIntensity > baseMBIntensity)
            {
                nowMBIntensity -= 0.07f* lastingDuration;
            }
            if (nowBloomIntensity > baseBloomIntensity)
            {
                nowBloomIntensity -= 0.07f* lastingDuration;
            }
            if (nowRadialBlurIntensity > baseRadialBlurIntensity)
            {
                nowRadialBlurIntensity -= 0.7f* lastingDuration;
            }
            if (nowParticleIntensity > baseParticleIntensity)
            {
                nowParticleIntensity -= 10f* lastingDuration;
            }
        }
        playerCamera.fieldOfView = nowFOV;
        _chromaticAberration.intensity.value = nowCAIntensity;
        _motionBlur.intensity.value = nowMBIntensity;
        _radialBlur.amount.value = nowRadialBlurIntensity;
        _bloom.intensity.value = nowBloomIntensity;
        _speedParticleEmission.rateOverTime = nowParticleIntensity;
    }
}