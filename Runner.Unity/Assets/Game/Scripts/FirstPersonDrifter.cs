// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// FirstPersonDrifter
using System;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;

public class FirstPersonDrifter : MonoBehaviour, ICharacterController
{
    public enum MovementType
    {
        Human,
        MechWalk,
        MechBoost,
        MechOverdrive
    }

    public class CappableJump
    {
        public float yVelocity;

        public CappableJump(float vel)
        {
            yVelocity = vel;
        }
    }

    public DrifterMovementMode[] modes;

    private Dictionary<MovementType, DrifterMovementMode> modeDict;

    private DrifterMovementMode currentMode;

    public float fallingDamageThreshold = 0.1f;

    public bool airControl = true;

    private int antiBunnyHopFactor;

    public float jumpForgivenessTime = 0.3f;

    private float jumpForgivenessTimer;

    public Vector3 airDrag = new Vector3(2f, 1f, 2f);

    public Vector3 groundDrag = new Vector3(5f, 0f, 5f);

    public Vector3 slipDrag = new Vector3(5f, 1f, 5f);

    public float slipDragAscend = 10f;

    public float inputRunThreshold = 0.5f;

    private Vector3 moveDirection = Vector3.zero;

    private Vector3 localMoveDirection;

    private bool grounded;

    private Transform myTransform;

    private RaycastHit hit;

    private float fallStartLevel;

    private bool falling;

    public float slideLimit;

    private bool playerControl = true;

    private int jumpTimer;

    public float groundCheckDistance = 0.3f;

    public LayerMask groundLayerMask;

    private Vector3 velocity;

    private Vector3 movementVelocity;

    public DamageableTrigger playerDashDamageableTrigger;

    private bool dashing;

    private float dashSpeed;

    private float dashTime;

    private float dashTimer;

    private Vector3 dashEndVelocity;

    private Vector3 dashDirection;

    private bool dashIsGodspeed;

    public int dashEnemyDamage = 10;

    public float m_dashEndingHangtimeVelocity = 1f;

    private bool stomping;

    public float stompSpeed;

    public float stompBumpUpSpeed;

    public float m_damageableStompDownwardForce = 10f;

    public float boostSpeed = 1f;

    private float gravityOverride;

    private bool m_ghostJumping;

    private Vector3 m_ghostJumpStart;

    private Vector3 m_ghostJumpEnd;

    private float m_ghostJumpTime;

    private float m_ghostJumpHeight;

    private float m_ghostJumpSpeed;

    private bool ziplining;

    public float ziplineSpeed;

    public float ziplineEndJumpSpeed = 33f;

    private MechController.ZiplinePoint currentZiplinePoint;

    private const float ZIPLINE_END_POP_DOT = -0.8f;

    public float ziplineBreakDamageRadius = 5f;

    public int ziplineBreakDamage = 2;

    public float parryBoostSpeed = 1f;

    public float parryUpForce = 1f;

    private float m_StepCycle;

    private float m_NextStep;

    public float m_StepInterval;

    public float m_slipSpeed = 6f;

    public float m_slipDot = 0.51f;

    public float m_noSlipDistance = 1f;

    [HideInInspector]
    public HeadBob headBob;

    public MouseLook mouseLookX;

    public MouseLook mouseLookY;

    private bool _waitForReleaseJump;

    public Zipline _zipline;

    private BaseDamageable _mountedDamageable;

    private KinematicCharacterMotor _motor;

    [HideInInspector]
    public Quaternion m_cameraRotationY = Quaternion.identity;

    [HideInInspector]
    public Quaternion m_cameraRotationX = Quaternion.identity;

    public Transform m_cameraHolder;

    [HideInInspector]
    public Quaternion m_motorRotation = Quaternion.identity;

    private CapsuleCollider _capsule;

    private float _moveStunAmount = 1f;

    public float moveStunRecoverySpeed = 1f;

    private const float ZIPLINE_EJECT_DISTANCE = 3f;

    private const float ZIPLINE_STUCK_RADIUS = 0.2f;

    private float ziplineStuckTimer;

    private const float ZIPLINE_STUCK_CANCEL_TIME = 0.2f;

    private bool _wasJumpHeldAtStartOfZipline;

    private Vector3 _lastZiplinePosition;

    private const float ZIPLINE_FORCE_BALLOON_POP_DIST = 1f;

    private Collider[] _colliderHits = new Collider[100];

    private Vector3 _lastPosition;

    private const float MOVEMENT_THRESHOLD = 0.1f;

    private CappableJump _lastCappableJump;

    public Action jumpAction;

    private bool _parryGivesSpeedBoost = true;

    private bool _parryPreventsFalling;

    private bool _parryCancelsDurationalMovement;

    private List<BaseDamageable> _dashTargets = new List<BaseDamageable>();

    private bool _isBoosting;

    public float m_boostMagnetismForce;

    private float inputX;

    private float inputY;

    private bool jumpDown;

    private bool jumpUp;

    private bool jumpHeld;

    private Vector3 _lastPos = Vector3.zero;

    private bool _noclip;

    private float fallTimer;

    private Vector3 _lastGroundedPosition;

    public float timeSinceLastTelefrag;

    private bool telefragging;

    private bool wasTelefragingLastFrame;

    private Vector3 telefragStartPosition;

    private Vector3 telefragEndPosition;

    public AnimationCurve telefragAnimCurve;

    public float telefragTime = 0.5f;

    public int telefragDamage = 8;

    private float telefragTimer;

    private const string SFX_TELEFRAG = "TELEFRAG";

    private const string SFX_TELEFRAG_END = "TELEFRAG_END";

    public float telefragSpeed = 10f;

    public float telefragUp = 10f;

    private BaseDamageable telefragTarget;

    public Vector3 LocalMoveDirection => localMoveDirection;

    public Vector3 Velocity => velocity;

    public Vector3 MovementVelocity => movementVelocity;

    public KinematicCharacterMotor Motor
    {
        get
        {
            if (_motor != null)
            {
                return _motor;
            }
            _motor = GetComponent<KinematicCharacterMotor>();
            return _motor;
        }
    }

    public CapsuleCollider Capsule
    {
        get
        {
            if (_capsule != null)
            {
                return _capsule;
            }
            _capsule = GetComponent<CapsuleCollider>();
            return _capsule;
        }
    }

    public DrifterMovementMode GetCurrentMovementMode()
    {
        return currentMode;
    }

    private float GetCurrentMoveSpeed()
    {
        return currentMode.moveSpeed * _moveStunAmount;
    }

    public void MoveStun()
    {
        _moveStunAmount = 0f;
    }

    private void Start()
    {
        Motor.CharacterController = this;
        _capsule = GetComponent<CapsuleCollider>();
        RM.drifter = this;
        myTransform = base.transform;
        jumpTimer = antiBunnyHopFactor;
        headBob = base.gameObject.GetComponentInChildren<HeadBob>();
        modeDict = new Dictionary<MovementType, DrifterMovementMode>();
        for (int i = 0; i < modes.Length; i++)
        {
            modeDict.Add(modes[i].type, modes[i]);
        }
        SetMovementType(MovementType.Human);
        m_NextStep = m_StepCycle / 2f;
        PlayerTeleport.TeleportToID("START");
        SetNoclip(noclip: false);
        playerDashDamageableTrigger.SetTrigger(active: false);
    }

    private void OnEnable()
    {
        Motor.OnPositionSet += OnMotorPositionSet;
        Motor.OnRotationSet += OnMotorRotationSet;
    }

    private void OnDisable()
    {
        Motor.OnPositionSet -= OnMotorPositionSet;
        Motor.OnRotationSet -= OnMotorRotationSet;
    }

    public void DoGhostJump(Vector3 targetPos, float height, float speed)
    {
        Debug.LogError("doing ghost jump");
        m_ghostJumpStart = base.transform.position;
        m_ghostJumpEnd = targetPos;
        m_ghostJumping = true;
        m_ghostJumpHeight = height;
        m_ghostJumpSpeed = speed;
        m_ghostJumpTime = 0f;
    }

    public void DoZipline(Vector3 startPosition, MechController.ZiplinePoint zPoint)
    {
        currentZiplinePoint = zPoint;
        if (stomping)
        {
            stomping = false;
            AudioController.Stop("ABILITY_STOMP_LOOP");
        }
        if (dashing)
        {
            dashing = false;
        }
        _zipline.SetZipline(active: true, animate: true, zPoint.point, (zPoint.point - startPosition).normalized);
        ziplining = true;
        _wasJumpHeldAtStartOfZipline = jumpHeld;
        grounded = false;
        Motor.ForceUnground();
        AudioController.Play("ZIPLINE_FIRE");
        AudioController.Play("ZIPLINE_LOOP");
        ziplineStuckTimer = 0f;
    }

    public void StopZipline(bool applyForce, float dot = 1f, float endDistance = float.MaxValue, bool wasManuallyCancelled = false)
    {
        if (!ziplining)
        {
            return;
        }
        ziplining = false;
        velocity = moveDirection;
        movementVelocity = Vector3.zero;
        moveDirection = Vector3.zero;
        if (applyForce && dot >= -0.8f)
        {
            velocity.y *= 0.5f;
            ForceJump(ziplineEndJumpSpeed, isCappable: false, cancelDurationalMovementAbilities: true, 0f);
        }
        RM.mechController.ForceStompAfterglow();
        AudioController.Stop("ZIPLINE_LOOP");
        AudioController.Play("ZIPLINE_END");
        _zipline.SetZipline(active: false, animate: true, Vector3.zero, Vector3.zero);
        if (endDistance < 1f)
        {
            Collider[] array = Physics.OverlapSphere(currentZiplinePoint.point, groundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < array.Length; i++)
            {
                EnemyBalloon component = array[i].GetComponent<EnemyBalloon>();
                if (array[i] != null && (bool)component && component.GetIsAlive())
                {
                    component.OnBalloonHit();
                }
            }
        }
        _zipline.OnBreak();
        if (currentZiplinePoint.damageable != null)
        {
            if (currentZiplinePoint.damageable.GetEnemyType() == Enemy.Type.balloon && currentZiplinePoint.damageable.GetDamageableType() == BaseDamageable.DamageableType.Enemy && currentZiplinePoint.damageable.GetIsAlive() && !wasManuallyCancelled)
            {
                currentZiplinePoint.damageable.GetComponent<EnemyBalloon>().OnBalloonHit();
                return;
            }
            if (currentZiplinePoint.damageable.GetEnemyType() == Enemy.Type.mimic && currentZiplinePoint.damageable.GetDamageableType() == BaseDamageable.DamageableType.Enemy && currentZiplinePoint.damageable.GetIsAlive() && EnemyMimic.mimicType == EnemyMimic.MimicType.Attack && !wasManuallyCancelled)
            {
                RM.mechController.OnHit(RM.mechController.currentHealth, currentZiplinePoint.damageable.transform.position, ignoreInvincibility: true);
            }
        }
        ProjectileBase.Explode(_colliderHits, currentZiplinePoint.point, ziplineBreakDamageRadius, ziplineBreakDamage, RM.mechController._stompLayerMask, ProjectileBase.DamageTarget.Damageable, base.gameObject, bombHit: false);
    }

    public void CancelZiplineFromAnotherAbility()
    {
        velocity = moveDirection;
        movementVelocity = Vector3.zero;
        moveDirection = Vector3.zero;
        ziplining = false;
        AudioController.Stop("ZIPLINE_LOOP");
        AudioController.Play("ZIPLINE_END");
        _zipline.SetZipline(active: false, animate: true, Vector3.zero, Vector3.zero);
        _zipline.OnBreak();
        ProjectileBase.Explode(_colliderHits, currentZiplinePoint.point, ziplineBreakDamageRadius, ziplineBreakDamage, RM.mechController._stompLayerMask, ProjectileBase.DamageTarget.Damageable, base.gameObject, bombHit: false);
    }

    public void SetGravityOverride(float newGravityOverride)
    {
        gravityOverride = newGravityOverride;
    }

    public void AddExternalVelocity(Vector3 vel)
    {
        moveDirection = Vector3.zero;
        movementVelocity = Vector3.zero;
        velocity += new Vector3(vel.x * airDrag.x, vel.y * airDrag.y, vel.z * airDrag.z);
    }

    public void AddVelocity(Vector3 vel)
    {
        moveDirection = Vector3.zero;
        movementVelocity = Vector3.zero;
        velocity += vel;
    }

    public void ProjectileVelocityBump()
    {
        velocity.y = Mathf.Max(velocity.y, 0f);
    }

    public bool GetIsGrounded()
    {
        return grounded;
    }

    public bool GetIsInsideJumpForgivenessWindow()
    {
        return jumpForgivenessTimer > 0f;
    }

    public bool GetIsMovingInSpace()
    {
        if (Vector3.Distance(_lastPosition, base.transform.position) > 0.1f)
        {
            return true;
        }
        return false;
    }

    public void ForceJump(float upwardVelocity, bool isCappable, bool cancelDurationalMovementAbilities, float cappableJumpModifier = 1f)
    {
        if (cancelDurationalMovementAbilities)
        {
            if (dashing)
            {
                moveDirection.y = 0f;
                dashing = false;
            }
            if (stomping)
            {
                stomping = false;
                AudioController.Stop("MECH_BOOST");
                AudioController.Stop("ABILITY_STOMP_LOOP");
            }
            if (ziplining)
            {
                StopZipline(applyForce: false);
            }
        }
        if (jumpAction != null)
        {
            jumpAction();
        }
        grounded = false;
        Motor.ForceUnground();
        if (_lastCappableJump != null)
        {
            velocity.y -= _lastCappableJump.yVelocity * cappableJumpModifier;
        }
        if (velocity.y < 0f)
        {
            velocity.y = 0f;
        }
        _ = velocity;
        velocity += Vector3.up * upwardVelocity;
        jumpTimer = 0;
        jumpForgivenessTimer = -1f;
        if (isCappable)
        {
            _lastCappableJump = new CappableJump(upwardVelocity);
        }
        else
        {
            _lastCappableJump = null;
        }
    }

    public void ForceFlap(float upwardVelocity)
    {
        if (dashing)
        {
            dashing = false;
        }
        grounded = false;
        Motor.ForceUnground();
        velocity += Vector3.up * upwardVelocity;
        jumpTimer = 0;
        AudioController.Play("WEAPON_RAPIER_FIRE");
    }

    public void SetMovementType(MovementType type)
    {
        currentMode = modeDict[type];
        Motor.SetCapsuleDimensions(currentMode.radius, 3.3f, 0f);
        headBob.midpoint = currentMode.height * 0.26f;
    }

    public void OnParry()
    {
        if (_parryCancelsDurationalMovement)
        {
            AudioController.Stop("MECH_BOOST");
            AudioController.Stop("ABILITY_STOMP_LOOP");
            dashing = false;
            stomping = false;
            StopZipline(applyForce: false, 0f);
        }
        if (_parryPreventsFalling && velocity.y <= 0f)
        {
            velocity.y = 0f;
        }
        if (_parryGivesSpeedBoost)
        {
            Vector3 vel = moveDirection * parryBoostSpeed;
            if (GetIsGrounded())
            {
                vel *= 2f;
            }
            AddVelocity(vel);
        }
    }

    public void ForceDash(float newDashSpeed, float newDashTime, Vector3 newDashDirection, Vector3 newDashEndVelocity, bool isGodspeed)
    {
        dashing = true;
        dashSpeed = newDashSpeed;
        dashTime = newDashTime;
        dashTimer = newDashTime;
        dashDirection = newDashDirection;
        dashEndVelocity = newDashEndVelocity;
        dashIsGodspeed = isGodspeed;
        if (isGodspeed)
        {
            AudioController.Play("MECH_DASH");
        }
        if (stomping)
        {
            stomping = false;
            AudioController.Stop("MECH_BOOST");
            AudioController.Stop("ABILITY_STOMP_LOOP");
        }
        if (ziplining)
        {
            StopZipline(applyForce: false);
        }
        _dashTargets.Clear();
    }

    public bool GetIsDashing()
    {
        return dashing;
    }

    private void LateUpdate()
    {
        ProgressStepCycle(GetCurrentMoveSpeed(), Time.deltaTime);
        _lastPosition = base.transform.position;
    }

    public void SetWaitForJumpRelease(bool j)
    {
        _waitForReleaseJump = j;
    }

    public void ForceStomp()
    {
        if (ziplining)
        {
            StopZipline(applyForce: false);
        }
        dashing = false;
        stomping = true;
        AudioController.Play("ABILITY_STOMP_ENGAGE");
        AudioController.Play("ABILITY_STOMP_LOOP");
    }

    public void ForceZeroVelocity()
    {
        velocity = Vector3.zero;
    }

    public bool GetIsStomping()
    {
        return stomping;
    }

    private void OnStompComplete(bool bumpUp)
    {
        stomping = false;
        AudioController.Stop("ABILITY_STOMP_LOOP");
        if (bumpUp)
        {
            velocity += Vector3.up * stompBumpUpSpeed;
        }
        moveDirection.y = 0f;
        RM.mechController.DoStompAbility();
    }

    public void SetBoost(bool newBoost)
    {
        _isBoosting = newBoost;
    }

    private void Update()
    {
        timeSinceLastTelefrag += Time.deltaTime;
        inputX = Singleton<GameInput>.Instance.GetAxis(GameInput.GameActions.MoveHorizontal);
        inputY = Singleton<GameInput>.Instance.GetAxis(GameInput.GameActions.MoveVertical);
        if (RM.mechController.GetIsInFireball())
        {
            float num = 1f - RM.mechController.GetFireballT();
            inputY += num * 2f;
            inputY = Mathf.Min(inputY, 1f);
        }
        inputX /= inputRunThreshold;
        inputY /= inputRunThreshold;
        if (inputX * inputX > 1f)
        {
            inputX = ((inputX < 0f) ? (-1f) : 1f);
        }
        if (inputY * inputY > 1f)
        {
            inputY = ((inputY < 0f) ? (-1f) : 1f);
        }
        jumpDown = Singleton<GameInput>.Instance.GetButtonDown(GameInput.GameActions.Jump, GameInput.InputType.Game);
        jumpUp = Singleton<GameInput>.Instance.GetButtonUp(GameInput.GameActions.Jump, GameInput.InputType.Game);
        jumpHeld = Singleton<GameInput>.Instance.GetButton(GameInput.GameActions.Jump);
        if (!RM.acceptInput || (RM.mechController != null && !RM.mechController.GetIsAlive()))
        {
            inputX = 0f;
            inputY = 0f;
        }
        if (_waitForReleaseJump && RM.acceptInput && !jumpDown && !jumpHeld)
        {
            _waitForReleaseJump = false;
        }
        if (_moveStunAmount < 1f)
        {
            _moveStunAmount = Mathf.Lerp(_moveStunAmount, 1f, moveStunRecoverySpeed * Time.deltaTime);
            if (Mathf.Approximately(_moveStunAmount, 1f))
            {
                _moveStunAmount = 1f;
            }
        }
        if (Application.isEditor)
        {
            Color color = (GetIsTelefragging() ? Color.blue : Color.red);
            color.a = 0.5f;
            Color color2 = (GetIsTelefragging() ? Color.blue : Color.red);
            Vector3 vector = base.transform.position - Vector3.up * _capsule.height * 0.5f;
            Debug.DrawRay(vector, Vector3.up * _capsule.height, color, 20f);
            if (_lastPos != Vector3.zero)
            {
                Debug.DrawLine(vector, _lastPos, color2, 20f);
                Debug.DrawLine(vector + Vector3.up * _capsule.height, _lastPos + Vector3.up * _capsule.height, color2, 20f);
            }
            _lastPos = vector;
        }
        m_cameraHolder.localRotation = m_cameraRotationY;
        m_cameraHolder.parent.localRotation = m_cameraRotationX;
    }

    public Vector3 GetFeetPosition()
    {
        return base.transform.position - Vector3.up * _capsule.height * 0.5f;
    }

    public void SetNoclip(bool noclip)
    {
        if (noclip && !_noclip)
        {
            ForceZeroVelocity();
            moveDirection = Vector3.zero;
            movementVelocity = Vector3.zero;
            velocity = Vector3.zero;
            Motor.BaseVelocity = Vector3.zero;
            Motor.SetCapsuleCollisionsActivation(collisionsActive: false);
            Motor.SetMovementCollisionsSolvingActivation(movementCollisionsSolvingActive: false);
            Motor.SetGroundSolvingActivation(stabilitySolvingActive: false);
            base.gameObject.SetLayer(17);
        }
        else if (!noclip && _noclip)
        {
            Motor.BaseVelocity = Vector3.zero;
            moveDirection = Vector3.zero;
            movementVelocity = Vector3.zero;
            velocity = Vector3.zero;
            ForceZeroVelocity();
            Motor.SetCapsuleCollisionsActivation(collisionsActive: true);
            Motor.SetMovementCollisionsSolvingActivation(movementCollisionsSolvingActive: true);
            Motor.SetGroundSolvingActivation(stabilitySolvingActive: true);
            base.gameObject.SetLayer(9);
        }
        _noclip = noclip;
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        wasTelefragingLastFrame = false;
        playerDashDamageableTrigger.SetTrigger(dashing || RM.mechController.GetIsInFireball());
        if (_noclip && (bool)RM.mechController && RM.mechController.GetIsAlive())
        {
            float num = ((inputX != 0f && inputY != 0f) ? 0.7071f : 1f);
            moveDirection.x = inputX * GetCurrentMoveSpeed() * num;
            moveDirection.z = inputY * GetCurrentMoveSpeed() * num;
            moveDirection.y = 0f;
            localMoveDirection = moveDirection;
            moveDirection = RM.mechController.playerCamera.transform.TransformDirection(moveDirection);
            movementVelocity += moveDirection;
            Vector3 vector = Vector3.ClampMagnitude(new Vector3(movementVelocity.x, movementVelocity.y, movementVelocity.z), currentMode.moveSpeedMax);
            movementVelocity.x = vector.x;
            movementVelocity.z = vector.z;
            movementVelocity.y = vector.y;
            movementVelocity.y /= 1f + currentMode.moveAcceleration * deltaTime;
            movementVelocity.x /= 1f + currentMode.moveAcceleration * deltaTime;
            movementVelocity.z /= 1f + currentMode.moveAcceleration * deltaTime;
            Vector3 vector2 = movementVelocity;
            if (jumpHeld)
            {
                vector2 *= 2f;
            }
            grounded = false;
            currentVelocity = vector2;
        }
        else if ((bool)RM.mechController && RM.mechController.GetIsAlive())
        {
            float num2 = ((inputX != 0f && inputY != 0f) ? 0.7071f : 1f);
            if (dashing)
            {
                Vector3 vector3 = dashDirection * AxKEasing.EaseOutQuad(dashSpeed * 0f, dashSpeed, dashTimer / dashTime);
                dashTimer -= 1f * deltaTime;
                moveDirection = vector3;
                playerControl = true;
                if (dashTimer <= 0f)
                {
                    dashing = false;
                    if (dashIsGodspeed)
                    {
                        AddExternalVelocity(dashEndVelocity);
                        velocity.y = m_dashEndingHangtimeVelocity;
                        if (!Motor.GetState().GroundingStatus.IsStableOnGround)
                        {
                            jumpTimer = 0;
                            jumpForgivenessTimer = -1f;
                        }
                    }
                    else
                    {
                        dashEndVelocity.y *= 2f;
                        dashEndVelocity.x *= 0.25f;
                        dashEndVelocity.z *= 0.25f;
                        AddExternalVelocity(moveDirection + dashEndVelocity);
                    }
                    moveDirection = Vector3.zero;
                }
            }
            else if (telefragging)
            {
                wasTelefragingLastFrame = true;
                inputX = 0f;
                inputY = 0f;
                playerControl = true;
                Motor.SetPosition(Vector3.Lerp(telefragStartPosition, telefragEndPosition, telefragAnimCurve.Evaluate(telefragTimer / telefragTime)));
                telefragTimer += 1f * deltaTime;
                timeSinceLastTelefrag = 0f;
                if (telefragTimer > telefragTime)
                {
                    OnTelefragEnd();
                }
            }
            else if (stomping)
            {
                inputX *= 0.1f;
                inputY *= 0.1f;
                moveDirection = new Vector3(0f, 0f - stompSpeed, 0f);
                bool flag = false;
                if (grounded)
                {
                    Collider[] array = Physics.OverlapSphere(Motor.transform.position - Vector3.up * (_capsule.height * 0.5f), groundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore);
                    bool flag2 = false;
                    for (int i = 0; i < array.Length; i++)
                    {
                        BaseDamageable component = array[i].GetComponent<BaseDamageable>();
                        if (component != null && component.GetDamageableType() == BaseDamageable.DamageableType.Enemy && component.GetEnemyType() == Enemy.Type.balloon)
                        {
                            component.GetComponent<EnemyBalloon>().OnBalloonHit();
                        }
                        if (component != null)
                        {
                            Debug.Log("Hit " + component.name);
                            Motor.ForceUnground();
                            grounded = false;
                            velocity.y -= m_damageableStompDownwardForce;
                            if (component.GetDamageableType() == BaseDamageable.DamageableType.Enemy && component.GetEnemyType() == Enemy.Type.mimic && EnemyMimic.mimicType == EnemyMimic.MimicType.Attack)
                            {
                                flag = true;
                            }
                            flag2 = true;
                            break;
                        }
                    }
                    OnStompComplete(!flag2);
                    if (flag)
                    {
                        RM.mechController.OnHit(RM.mechController.currentHealth, RM.playerPosition, ignoreInvincibility: true);
                    }
                    return;
                }
            }
            else if (ziplining)
            {
                inputX = 0f;
                inputY = 0f;
                playerControl = true;
                Vector3 normalized = (currentZiplinePoint.point - base.transform.position).normalized;
                moveDirection = normalized * ziplineSpeed;
                float dot = Vector3.Dot(Vector3.up, normalized);
                float num3 = Vector3.Distance(currentZiplinePoint.point, base.transform.position);
                if ((bool)currentZiplinePoint.damageable && !currentZiplinePoint.damageable.GetIsAlive())
                {
                    StopZipline(applyForce: true, dot, num3);
                }
                if (num3 < 3f)
                {
                    StopZipline(applyForce: true, dot, num3);
                }
                else if (!_wasJumpHeldAtStartOfZipline && jumpHeld)
                {
                    StopZipline(applyForce: true, dot, num3, wasManuallyCancelled: true);
                }
                else
                {
                    if (_wasJumpHeldAtStartOfZipline && !jumpHeld)
                    {
                        _wasJumpHeldAtStartOfZipline = false;
                    }
                    if (Vector3.Distance(_lastZiplinePosition, base.transform.position) < 0.2f)
                    {
                        ziplineStuckTimer += 1f * deltaTime;
                        if (ziplineStuckTimer > 0.2f)
                        {
                            StopZipline(applyForce: true, dot, num3, wasManuallyCancelled: true);
                        }
                    }
                    else
                    {
                        ziplineStuckTimer = 0f;
                    }
                }
                Motor.ForceUnground();
                _lastZiplinePosition = base.transform.position;
            }
            else if (m_ghostJumping)
            {
                inputX = 0f;
                inputY = 0f;
                _capsule.enabled = false;
                float num4 = currentMode.moveSpeedMax * 1.3f / (m_ghostJumpEnd - m_ghostJumpStart).magnitude;
                m_ghostJumpTime += Time.deltaTime * num4 * m_ghostJumpSpeed;
                moveDirection = (GhostJump.Parabola(m_ghostJumpStart, m_ghostJumpEnd, m_ghostJumpHeight, Mathf.Clamp01(m_ghostJumpTime)) - base.transform.position) * (1f / Time.deltaTime);
                RM.drifter.mouseLookX.SetRotationX(57.29578f * Mathf.Atan2(moveDirection.x, moveDirection.z));
                if (m_ghostJumpTime >= 1f)
                {
                    m_ghostJumping = false;
                    _capsule.enabled = true;
                }
                Motor.ForceUnground();
            }
            else if (grounded)
            {
                Collider[] array2 = Physics.OverlapSphere(Motor.transform.position - Vector3.up * (_capsule.height * 0.5f), groundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore);
                _mountedDamageable = null;
                for (int j = 0; j < array2.Length; j++)
                {
                    if (array2[j].CompareTag("Boost"))
                    {
                        RM.mechController.OnHitBooster();
                    }
                    _mountedDamageable = array2[j].GetComponent<BaseDamageable>();
                    if (!(array2[j] != null) || !(_mountedDamageable != null))
                    {
                        continue;
                    }
                    if (_mountedDamageable.GetDamageableType() == BaseDamageable.DamageableType.Platform)
                    {
                        ((BreakablePlatform)_mountedDamageable).OnPlayerMount();
                    }
                    else
                    {
                        BreakablePlatform breakablePlatform = _mountedDamageable.GetBreakablePlatform();
                        if (breakablePlatform != null)
                        {
                            breakablePlatform.OnPlayerMount();
                        }
                    }
                    if (_mountedDamageable.GetDamageableType() == BaseDamageable.DamageableType.Enemy && _mountedDamageable.GetEnemyType() == Enemy.Type.balloon)
                    {
                        ((EnemyBalloon)_mountedDamageable).OnBalloonHit();
                    }
                }
                if (falling)
                {
                    falling = false;
                    if (currentMode.type != 0 && fallTimer > fallingDamageThreshold)
                    {
                        FallingDamageAlert(fallStartLevel - myTransform.position.y);
                        fallTimer = 0f;
                    }
                }
                moveDirection = new Vector3(inputX * GetCurrentMoveSpeed() * num2, 0f, inputY * GetCurrentMoveSpeed() * num2);
                localMoveDirection = moveDirection;
                moveDirection = m_cameraHolder.parent.TransformDirection(moveDirection);
                if (velocity.y <= 0f)
                {
                    velocity.y = 0f;
                }
                if (grounded && !_waitForReleaseJump)
                {
                    if (!jumpHeld)
                    {
                        jumpTimer++;
                    }
                    else if (jumpTimer >= antiBunnyHopFactor && RM.acceptInput)
                    {
                        ForceJump(currentMode.jumpSpeed, isCappable: true, cancelDurationalMovementAbilities: false);
                        if (RM.mechController.GetIsBoosting() && !RM.mechController.GetIsInFireball())
                        {
                            AudioController.Play("MECH_JUMP_WATER");
                        }
                        else
                        {
                            AudioController.Play("MECH_JUMP");
                        }
                    }
                }
            }
            else
            {
                if (Physics.CheckSphere(Motor.transform.position + Vector3.up * (_capsule.height * 0.5f + _capsule.radius), groundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore) && velocity.y > 0f)
                {
                    velocity.y *= 0.9f;
                }
                if (!falling)
                {
                    falling = true;
                    fallStartLevel = myTransform.position.y;
                    fallTimer = 0f;
                    if (jumpTimer > 0)
                    {
                        jumpForgivenessTimer = jumpForgivenessTime;
                    }
                }
                else
                {
                    fallTimer += 1f * deltaTime;
                }
                if (jumpForgivenessTimer > 0f && ((RM.acceptInput && jumpDown) || jumpUp))
                {
                    ForceJump(currentMode.jumpSpeed, isCappable: true, cancelDurationalMovementAbilities: false);
                    if (RM.mechController.GetIsBoosting())
                    {
                        AudioController.Play("MECH_JUMP_WATER");
                    }
                    else
                    {
                        AudioController.Play("MECH_JUMP");
                    }
                }
                jumpForgivenessTimer -= 1f * deltaTime;
                if (playerControl)
                {
                    moveDirection.x = inputX * GetCurrentMoveSpeed() * num2;
                    if (inputY > 0f)
                    {
                        moveDirection.z = inputY * GetCurrentMoveSpeed() * num2;
                    }
                    else
                    {
                        moveDirection.z = inputY * GetCurrentMoveSpeed() * num2;
                    }
                    localMoveDirection = moveDirection;
                    moveDirection = m_cameraHolder.parent.TransformDirection(moveDirection);
                }
            }
            if (dashing)
            {
                movementVelocity.y = 0f;
                movementVelocity = moveDirection;
            }
            else if (telefragging)
            {
                movementVelocity = Vector3.zero;
            }
            else if (stomping)
            {
                movementVelocity = moveDirection;
            }
            else if (ziplining)
            {
                movementVelocity = moveDirection;
            }
            else if (m_ghostJumping)
            {
                movementVelocity = moveDirection;
            }
            else
            {
                movementVelocity.y = 0f;
                movementVelocity += moveDirection;
                Vector3 vector4 = Vector3.ClampMagnitude(new Vector3(movementVelocity.x, 0f, movementVelocity.z), currentMode.moveSpeedMax);
                movementVelocity.x = vector4.x;
                movementVelocity.z = vector4.z;
                if (Mathf.Abs(inputX) < 0.1f && Mathf.Abs(inputY) < 0.1f)
                {
                    movementVelocity.x *= 1f / (1f + currentMode.moveAcceleration * 10f * deltaTime);
                    movementVelocity.z *= 1f / (1f + currentMode.moveAcceleration * 10f * deltaTime);
                }
                else
                {
                    movementVelocity.x *= 1f / (1f + currentMode.moveAcceleration * deltaTime);
                    movementVelocity.z *= 1f / (1f + currentMode.moveAcceleration * deltaTime);
                }
            }
            _ = groundCheckDistance;
            if (stomping)
            {
                _ = groundCheckDistance;
            }
            bool flag3 = grounded;
            grounded = Motor.GetState().GroundingStatus.IsStableOnGround;
            if (telefragging || m_ghostJumping)
            {
                grounded = false;
                Motor.ForceUnground();
            }
            bool flag4 = false;
            if (grounded && velocity.y > 0f)
            {
                grounded = false;
                Motor.ForceUnground();
            }
            if (Time.timeSinceLevelLoad > 0.1f && grounded && !flag3)
            {
                RM.ghostRecorder.RecordLand();
            }
            if (dashing || (stomping && !flag4) || ziplining || telefragging || m_ghostJumping)
            {
                velocity = Vector3.zero;
            }
            else if (stomping && flag4)
            {
                velocity.x /= 1f + slipDrag.x * deltaTime;
                velocity.y = 0f;
                velocity.z /= 1f + slipDrag.z * deltaTime;
            }
            else
            {
                if (_isBoosting)
                {
                    velocity += movementVelocity * boostSpeed * deltaTime;
                    if (velocity.y <= 0f)
                    {
                        velocity.y += m_boostMagnetismForce * deltaTime;
                    }
                }
                float gravity = currentMode.gravity;
                if (gravityOverride != 0f)
                {
                    gravity = gravityOverride;
                }
                velocity.y -= gravity * deltaTime;
                if (grounded)
                {
                    velocity.y = 0f;
                    if (velocity.sqrMagnitude > 2f)
                    {
                        velocity.x /= 1f + groundDrag.x * deltaTime;
                        velocity.y /= 1f + groundDrag.y * deltaTime;
                        velocity.z /= 1f + groundDrag.z * deltaTime;
                    }
                    else
                    {
                        velocity.x /= 1f + groundDrag.x * 20f * deltaTime;
                        velocity.y /= 1f + groundDrag.y * 20f * deltaTime;
                        velocity.z /= 1f + groundDrag.z * 20f * deltaTime;
                    }
                }
                else if (flag4)
                {
                    velocity.x /= 1f + slipDrag.x * deltaTime;
                    velocity.y /= 1f + slipDrag.y * deltaTime;
                    velocity.z /= 1f + slipDrag.z * deltaTime;
                    if (velocity.y > 0f)
                    {
                        velocity.y /= 1f + slipDragAscend * deltaTime;
                    }
                    else
                    {
                        velocity.y /= 1f + slipDrag.y * deltaTime;
                    }
                }
                else
                {
                    velocity.x /= 1f + airDrag.x * deltaTime;
                    velocity.y /= 1f + airDrag.y * deltaTime;
                    velocity.z /= 1f + airDrag.z * deltaTime;
                }
            }
            currentVelocity = velocity + movementVelocity;
            if (grounded)
            {
                _lastGroundedPosition = base.transform.position - Vector3.up * (_capsule.height * 0.5f);
            }
        }
        else
        {
            currentVelocity = Vector3.zero;
        }
    }

    public Vector3 GetLastGroundedPosition()
    {
        return _lastGroundedPosition;
    }

    public void OnPlayerDie()
    {
        if (ziplining)
        {
            StopZipline(applyForce: false);
        }
        if (stomping)
        {
            stomping = false;
            AudioController.Stop("MECH_BOOST");
            AudioController.Stop("ABILITY_STOMP_LOOP");
        }
        if (dashing)
        {
            dashing = false;
        }
    }

    private void ProgressStepCycle(float speed, float deltaTime)
    {
        if (GetIsMovingInSpace() && movementVelocity.magnitude > 0f)
        {
            float num = (movementVelocity.magnitude + speed * (1f / GetCurrentMoveSpeed())) * deltaTime;
            m_StepCycle += num;
        }
        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }
        m_NextStep = m_StepCycle + m_StepInterval;
        if (grounded)
        {
            if (RM.mechController.GetIsBoosting())
            {
                AudioController.Play("MECH_FOOTSTEP_WATER");
            }
            else
            {
                AudioController.Play("MECH_FOOTSTEP");
            }
        }
    }

    private void FallingDamageAlert(float fallDistance)
    {
        headBob.ShakeDirection(Vector3.down, Mathf.Clamp(fallDistance, 0.6f, 2f));
        if (RM.mechController.GetIsBoosting())
        {
            AudioController.Play("MECH_LAND_WATER");
        }
        else
        {
            AudioController.Play("MECH_LAND");
        }
    }

    public void Teleport(Vector3 position, bool preservePlayerVelocity)
    {
        Motor.SetPosition(position + Vector3.up * (_capsule.height * 0.5f));
        ForceZeroVelocity();
        moveDirection = Vector3.zero;
    }

    public BaseDamageable GetCurrentTelefragTarget()
    {
        return telefragTarget;
    }

    public void Telefrag(BaseDamageable damageable)
    {
        if (!telefragging)
        {
            telefragging = true;
            telefragStartPosition = base.transform.position;
            telefragEndPosition = damageable.transform.position;
            RM.exploder.TelefragTarget(damageable.transform.position, damageable.GetComponent<Collider>().bounds.extents.magnitude * 0.5f);
            telefragTimer = 0f;
            telefragTarget = damageable;
            telefragTarget.OnTelefragStart();
            if (ziplining)
            {
                CancelZiplineFromAnotherAbility();
            }
            if (stomping)
            {
                stomping = false;
                AudioController.Stop("MECH_BOOST");
                AudioController.Stop("ABILITY_STOMP_LOOP");
            }
            if (dashing)
            {
                dashing = false;
            }
            AudioController.Play("TELEFRAG");
        }
        else
        {
            Debug.LogWarning("Attempted a telefrag while we are already telefragging");
        }
    }

    private void OnTelefragEnd()
    {
        bool flag = false;
        if (telefragTarget.GetDamageableType() == BaseDamageable.DamageableType.Enemy && telefragTarget.GetEnemyType() == Enemy.Type.mimic && EnemyMimic.mimicType == EnemyMimic.MimicType.Attack)
        {
            flag = true;
        }
        telefragging = false;
        if (!flag)
        {
            telefragTarget.SetDieSFX("");
        }
        Vector3 normalized = (telefragEndPosition - telefragStartPosition).normalized;
        float num = 1f;
        bool flag2 = true;
        int damage = telefragDamage;
        float num2 = 0f;
        if (telefragTarget.GetDamageableType() == BaseDamageable.DamageableType.CrystalExplosive || telefragTarget.GetEnemyType() == Enemy.Type.bossBasic)
        {
            Vector3 vector = telefragStartPosition;
            vector.y = telefragEndPosition.y;
            normalized = (vector - telefragEndPosition).normalized;
            normalized.y = 0.6f;
            num = 1.5f;
            flag2 = false;
            num2 = 1f;
            damage = ((telefragTarget.GetEnemyType() != Enemy.Type.bossBasic) ? telefragTarget.maxHealth : 30);
        }
        AudioController.Play("TELEFRAG_END");
        base.transform.position = telefragEndPosition;
        ForceZeroVelocity();
        moveDirection = Vector3.zero;
        Vector3 vel = normalized * telefragSpeed * num;
        vel.y *= num2;
        AddVelocity(vel);
        if (flag2)
        {
            ForceJump(telefragUp, isCappable: false, cancelDurationalMovementAbilities: true);
        }
        telefragTarget.OnHit(base.transform.position, damage, BaseDamageable.DamageSource.Dash);
        if (flag)
        {
            if (!GameDataManager.saveData.playerAchievementData.bookOfLifeMimicDeath)
            {
                GameDataManager.saveData.playerAchievementData.bookOfLifeMimicDeath = true;
                GameDataManager.SaveGame();
            }
            Achievements.SyncBookOfLifeMimicAchievement(storeStats: true);
            RM.mechController.OnHit(RM.mechController.currentHealth, telefragTarget.transform.position, ignoreInvincibility: true);
        }
        telefragTarget.OnTelefragEnd();
        telefragTarget = null;
        timeSinceLastTelefrag = 0f;
        RM.mechController.TriggerInvincibilityTimer();
    }

    public void CancelTelefrag()
    {
        if (telefragging)
        {
            telefragging = false;
            AudioController.Play("TELEFRAG_END");
            telefragTarget = null;
        }
    }

    public bool GetIsTelefragging()
    {
        return telefragging;
    }

    public bool WasTelefraging()
    {
        return telefragging;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Motor.transform.position - Vector3.up * (Capsule.height * 0.5f - Capsule.radius * 0.9f), groundCheckDistance);
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        RM.mechController.GetIsAlive();
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        if (hitCollider.CompareTag("Boost"))
        {
            RM.mechController.OnHitBooster();
        }
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        if (hitCollider.gameObject != null)
        {
            BaseDamageable component = hitCollider.gameObject.GetComponent<BaseDamageable>();
            OnMovementHitDamageable(component);
        }
    }

    public void OnMovementHitDamageable(BaseDamageable dmg)
    {
        if ((bool)dmg && dmg.GetDamageableType() == BaseDamageable.DamageableType.Enemy && dmg.GetEnemyType() == Enemy.Type.balloon)
        {
            dmg.gameObject.GetComponent<EnemyBalloon>().OnBalloonHit();
            if (stomping)
            {
                OnStompComplete(bumpUp: false);
            }
        }
        if (dashing && (bool)dmg && (dmg.GetDamageableType() != BaseDamageable.DamageableType.Enemy || dmg.GetEnemyType() != Enemy.Type.balloon) && !_dashTargets.Contains(dmg))
        {
            int num = dashEnemyDamage;
            if (dmg.GetDamageableType() == BaseDamageable.DamageableType.Enemy && dmg.GetEnemyType() == Enemy.Type.bossBasic)
            {
                num /= 2;
            }
            dmg.OnHit(hit.point, num, BaseDamageable.DamageSource.Dash);
            RM.mechController.ForceStompAfterglow();
            _dashTargets.Add(dmg);
        }
        if (RM.mechController.GetIsInFireball() && (bool)dmg && (dmg.GetDamageableType() != BaseDamageable.DamageableType.Enemy || dmg.GetEnemyType() != Enemy.Type.balloon))
        {
            int num2 = dashEnemyDamage;
            if (dmg.GetDamageableType() == BaseDamageable.DamageableType.Enemy && dmg.GetEnemyType() == Enemy.Type.bossBasic)
            {
                num2 /= 2;
            }
            dmg.OnHit(hit.point, num2, BaseDamageable.DamageSource.Dash);
            RM.mechController.ForceStompAfterglow();
        }
        if ((dashing || RM.mechController.GetIsInFireball() || stomping) && (bool)dmg && dmg.GetDamageableType() == BaseDamageable.DamageableType.Enemy && dmg.GetEnemyType() == Enemy.Type.mimic && EnemyMimic.mimicType == EnemyMimic.MimicType.Attack)
        {
            if (stomping)
            {
                OnStompComplete(bumpUp: false);
            }
            RM.mechController.OnHit(RM.mechController.currentHealth, dmg.transform.position, ignoreInvincibility: true);
        }
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    private void OnMotorPositionSet(Vector3 position)
    {
        RM.playerPosition = position;
    }

    private void OnMotorRotationSet(Quaternion rotation)
    {
        RM.playerRotation = rotation;
    }
}
