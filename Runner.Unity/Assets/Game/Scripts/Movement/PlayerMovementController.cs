using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public enum AbilityType { Base, AirJump, Dash, Stomp }
public enum MovementState { Running, Dashing, Air }

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float playerVelocity;
    private Vector3 myVelocity;
    private Rigidbody _rb;

    private ReactiveProperty<MovementState> _state;
    public IReactiveProperty<MovementState> State => _state;
    private MovementState lastState;

    private float _playerRadius;
    private bool _isGrounded;
    private bool _keepMomentum;
    [SerializeField] private bool _hasDrag;

    [Space][Header("Movement")]
    [SerializeField] private float acceleration = 13f;
    [SerializeField] private float deceleration = 13f;
    [SerializeField] private float _maxSpeed = 15f;
    [SerializeField] private float maxYSpeed = 15f;
    [SerializeField] private float gravity = 2.5f;
    [Space]

    private float _coyoteTime = 0.2f;
    private float _coyoteTimeCounter;
    private float _jumpBufferTime = 0.2f;
    private float _jumpBufferCounter;
    private float _distance;
    private float _ySpeedLimit;
    private float _moveSpeed;
    private float _desiredMoveSpeed;
    private float _lastDesiredMoveSpeed;
    private float _xInput;
    private float _yInput;
    private Vector3 _moveDirection;

    [Space][Header("Ability")]
    [SerializeField] private KeyCode abilityKey;
    [SerializeField] private KeyCode swapKey;
    [SerializeField] private KeyCode executeKey = KeyCode.Mouse0;
    private Subject<Vector3> _onExecuteInput;
    public IObservable<Vector3> OnExecuteInputObservable => _onExecuteInput;

    public AbilityType currentAbility;
    public AbilityType secondaryAbility;
    public InventoryDictionary<AbilityType, int> inventory = new()
    {
        { AbilityType.Base, 0 },
        { AbilityType.AirJump, 0 },
        { AbilityType.Dash, 0 },
        { AbilityType.Stomp, 0 },
    };

    private int _currentValue;

    [Space][Header("Jump")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private float jumpForce = 25;
    [SerializeField] private float jumpCooldown = 0.25f;
    private bool _canJump = true;

    [Space][Header("Dash")]
    [SerializeField] private float dashForce = 150f;
    [SerializeField] private float dashSpeedChangeFactor = 50f;
    [SerializeField] private float dashDuration = 1f;
    private float _dashTimer;
    private float _speedChangeFactor;
    private Vector3 _dashDirection;
    private Vector3 _dashSpeed;
    private bool _isDashing = false;

    private void Awake()
    {
        _state = new(MovementState.Running);
        _onExecuteInput = this.UpdateAsObservable().Where(_ => Input.GetKeyDown(executeKey))
                                                             .ThrottleFirst(TimeSpan.FromSeconds(0.5f));
        
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _playerRadius = GetComponent<CapsuleCollider>().radius;
        _distance = (_playerRadius * 1.414f) + 0.5f;
    }

    private void Update()
    {
        playerVelocity = new Vector3(_rb.velocity.x , 0f, _rb.velocity.z).magnitude;
        CheckGrounded();
        StateHandler();
        MyInput();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        UpdateVelocity();
    }

    /// <summary>
    /// Check if there is an object below player model
    /// </summary>
    private void CheckGrounded()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * 0.5f - 0.5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        if (Physics.Raycast(origin, direction, out RaycastHit hit, _distance))
        {
            Debug.DrawRay(origin, direction * _distance, Color.red);
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void MyInput()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");
        float num = ((_xInput != 0f && _yInput != 0f) ? 0.7071f : 1f);

        if (Input.GetKeyDown(jumpKey))
        {
            _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if (_jumpBufferCounter > 0 && _canJump && _coyoteTimeCounter > 0)
        {
            _canJump = false;
            _jumpBufferCounter = 0;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (!_canJump)
        {
            _coyoteTimeCounter = 0;
        }

        if (Input.GetKeyDown(abilityKey))
        {
            UseAbility();
        }

        if (Input.GetKeyDown(swapKey))
        {
            SwapInventory();
        }

        if (Input.GetKeyDown(executeKey))
        {
            _onExecuteInput.OnNext(transform.position);
            Execute();
        }
    }

    private void StateHandler()
    {
        if (_isDashing)
        {
            _state.Value = MovementState.Dashing;
            _hasDrag = false;
            _desiredMoveSpeed = dashForce * 10f;
            _speedChangeFactor = dashSpeedChangeFactor;
        }

        else if (_isGrounded && !_isDashing)
        {
            _state.Value = MovementState.Running;
            _hasDrag = true;
            _coyoteTimeCounter = _coyoteTime;
            _desiredMoveSpeed = acceleration;
        }

        else if (!_isGrounded && !_isDashing)
        {
            _state.Value = MovementState.Air;
            _hasDrag = true;
            _coyoteTimeCounter -= Time.deltaTime;
            _desiredMoveSpeed = acceleration;
        }

        bool desiredMoveSpeedChanged = _desiredMoveSpeed != _lastDesiredMoveSpeed;
        if (lastState == MovementState.Dashing)
        {
            _keepMomentum = true;
        }
        if (desiredMoveSpeedChanged)
        {
            if (_keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(LerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                _moveSpeed = _desiredMoveSpeed;
            }
        }

        _lastDesiredMoveSpeed = _desiredMoveSpeed;
        lastState = _state;
    }

    private void UpdateVelocity()
    {
        _moveDirection = transform.forward * _yInput + transform.right * _xInput;

        if (_state.Value == MovementState.Dashing)
        {
            _dashSpeed =  _dashDirection * EaseOutQuad(dashForce * 0f * 10f, dashForce * 10f, _dashTimer / dashDuration);
            _dashTimer -= 1f * Time.deltaTime;

            _rb.AddForce(_dashSpeed, ForceMode.Impulse);
        }

        // on ground running
        else if(_state.Value == MovementState.Running)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f);
        }

        // in air
        else if(_state.Value == MovementState.Air)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f);
            _rb.AddForce(Physics.gravity * (gravity - 1) * _rb.mass);
        }

        if (_hasDrag)
        {
            // x-axis, z-axis drag calculation
            if (Mathf.Abs(_yInput) < 0.1f && Mathf.Abs(_xInput) < 0.1f)
            {
                Vector3 direction = GetDirection(cameraTransform);
                myVelocity.x *= 1f / (1f + deceleration * 10f * Time.deltaTime);
                myVelocity.z *= 1f / (1f + deceleration * 10f * Time.deltaTime);
                _rb.velocity = new Vector3(myVelocity.x, _rb.velocity.y, myVelocity.z);
            }
            else
            {
                myVelocity.x *= 1f / (1f + deceleration * Time.deltaTime);
                myVelocity.z *= 1f / (1f + deceleration * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Limits player speed
    /// </summary>
    private void SpeedControl()
    {
        Vector3 _flatVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        if(_flatVelocity.magnitude > _maxSpeed)
        {
            Vector3 _limitedVelocity = _flatVelocity.normalized * _maxSpeed;
            _rb.velocity = new Vector3(_limitedVelocity.x, _rb.velocity.y, _limitedVelocity.z);
        }

        if (_ySpeedLimit != 0 && _rb.velocity.y > _ySpeedLimit)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _ySpeedLimit, _rb.velocity.z);
        }
    }

    public static float EaseOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }

    /// <summary>
    /// Exponentially reduce given speed to target speed
    /// </summary>
    private IEnumerator LerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(_desiredMoveSpeed - _moveSpeed);
        float startValue = _moveSpeed;

        float boostFactor = _speedChangeFactor;

        while (time < difference)
        {
            _moveSpeed = Mathf.Lerp(startValue, _desiredMoveSpeed, time / difference);
            time += Time.deltaTime * boostFactor * 10f;

            yield return null;
        }

        _moveSpeed = _desiredMoveSpeed;
        _speedChangeFactor = 1f;
        _keepMomentum = false;
    }

    /// <summary>
    /// Completely stops the player
    /// </summary>
    private void ResetMomentum()
    {
        _rb.isKinematic = true;
        _rb.isKinematic = false;
    }

    /// <summary>
    /// Use current ability if applicable
    /// </summary>
    public void UseAbility()
    {
        _currentValue = inventory.GetValueOrDefault(currentAbility);
        switch (currentAbility)
        {
            case AbilityType.AirJump:
                if (_currentValue > 0 && _canJump && !_isGrounded)
                {
                    AirJump();
                    ConsumeInventory(currentAbility, _currentValue);
                }
                break;

            case AbilityType.Dash:
                if (_currentValue > 0 && !_isDashing)
                {
                    Dash();
                    ConsumeInventory(currentAbility, _currentValue);
                }
                break;

            case AbilityType.Stomp:
                if (_currentValue > 0)
                {
                    Stomp();
                    ConsumeInventory(currentAbility, _currentValue);
                }
                break;
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void AirJump()
    {
        _canJump = false;
        Jump();

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        _canJump = true;
    }

    private void Dash()
    {
        _isDashing = true;
        _rb.useGravity = false;
        _dashTimer = dashDuration;
        _ySpeedLimit = maxYSpeed;
        _dashDirection = GetDirection(transform);
        ResetMomentum();
        Invoke(nameof(ResetDash), dashDuration);
    }

    private void ResetDash()
    {
        _isDashing = false;
        _rb.useGravity = true;
        _dashSpeed = Vector3.zero;
        _ySpeedLimit = 0;
    }

    /// <summary>
    /// Return where the player is facing as a Vector3 value (excluding y-axis)
    /// </summary>
    private Vector3 GetDirection(Transform forwardTransform)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = forwardTransform.forward;

        if (verticalInput == 0 && horizontalInput == 0)
        {
            direction = forwardTransform.forward;
        }

        return direction.normalized;
    }

    private void Stomp()
    {
        // Stomp
    }

    private void Execute()
    {
        Debug.Log($"Execute called from: {gameObject.name}");
    }

    /// <summary>
    /// Reduce current ability count by 1
    /// </summary>
    public void ConsumeInventory(AbilityType type, int value)
    {
        value -= 1;
        inventory[type] = value;

        if (value == 0)
        {
            currentAbility = secondaryAbility;
            secondaryAbility = AbilityType.Base;
        }
    }

    /// <summary>
    /// Change current ability and secondary ability
    /// </summary>
    public void SwapInventory()
    {
        if (secondaryAbility != AbilityType.Base) 
        {
            AbilityType tempAbility = currentAbility;
            currentAbility = secondaryAbility;
            secondaryAbility = tempAbility;
        }
    }
}
