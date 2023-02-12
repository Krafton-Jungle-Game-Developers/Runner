using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType { Base, ExtraJump, Dash, Stomp }
public enum MovementState { Running, Dashing, Air }

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    private Rigidbody _rb;
    private Collider _collider;

    private MovementState state;
    private MovementState lastState;

    private float _playerRadius;
    [SerializeField] private bool _isGrounded;
    private bool _keepMomentum;

    [Space][Header("Movement")]
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float maxYSpeed;
    [SerializeField] private float gravity;
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
    [SerializeField] public AbilityType currentAbility = AbilityType.Base;

    public InventoryDictionary<AbilityType, int> inventory = new()
    {
        { AbilityType.Base, 0 },
        { AbilityType.ExtraJump, 0 },
        { AbilityType.Dash, 0 },
        { AbilityType.Stomp, 0 },
    };

    private int _currentValue;

    [Space][Header("Jump")]
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    private bool _canJump = true;

    [Space][Header("Dash")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashSpeedChangeFactor;
    [SerializeField] private float dashCooldown;
    private float _speedChangeFactor;
    private Vector3 _delayedForce;
    private bool _isDashing = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _rb.freezeRotation = true;
        _playerRadius = GetComponent<CapsuleCollider>().radius;
        _distance = (_playerRadius * 1.414f) + 0.5f;
    }

    private void Update()
    {
        CheckGrounded();
        StateHandler();
        MyInput();
        SpeedControl();
        if(!_canJump)
        {
            _coyoteTimeCounter = 0;
        }

        if (Input.GetKeyDown(jumpKey))
        {
            _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Shoot a raycast and check if there is a object below player model
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

        if (_jumpBufferCounter > 0 && _canJump && _coyoteTimeCounter > 0)
        {
            _canJump = false;
            _jumpBufferCounter = 0;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(abilityKey))
        {
            UseAbility();
        }
    }

    private void StateHandler()
    {
        if (_isDashing)
        {
            state = MovementState.Dashing;
            _desiredMoveSpeed = dashForce;
            _speedChangeFactor = dashSpeedChangeFactor;
        }

        else if (_isGrounded)
        {
            state = MovementState.Running;
            _coyoteTimeCounter = _coyoteTime;
            _desiredMoveSpeed = acceleration;
        }

        else if (!_isGrounded)
        {
            state = MovementState.Air;
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
        lastState = state;
    }
    private void MovePlayer()
    {
        _moveDirection = transform.forward * _yInput + transform.right * _xInput;
        Vector3 _xyVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        if (_isDashing)
        {
            _moveSpeed = dashForce;
        }
        // on ground
        if(_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * Mathf.Sqrt(deceleration) * 10f);
        }

        // in air
        else if(!_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * Mathf.Sqrt(deceleration) * 10f);
            _rb.AddForce(Physics.gravity * (gravity - 1) * _rb.mass);
        }

        if (_yInput == 0 || _xInput == 0)
        {
            _rb.AddForce(_xyVelocity * -deceleration);
        }
    }

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

    private IEnumerator LerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(_desiredMoveSpeed - _moveSpeed);
        float startValue = _moveSpeed;

        float boostFactor = _speedChangeFactor;

        while (time < difference)
        {
            _moveSpeed = Mathf.Lerp(startValue, _desiredMoveSpeed, time / difference);
            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        _moveSpeed = _desiredMoveSpeed;
        _speedChangeFactor = 1f;
        _keepMomentum = false;
    }

    public void UseAbility()
    {
        switch (currentAbility)
        {
            case AbilityType.ExtraJump:
                AirJump();
                break;

            case AbilityType.Dash:
                Dash();
                break;

            case AbilityType.Stomp:
                Stomp();
                break;
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _canJump = true;
    }

    private void AirJump()
    {
        _currentValue = inventory.GetValueOrDefault(AbilityType.ExtraJump);
        if (_currentValue > 0 && _canJump && !_isGrounded)
        {
            _canJump = false;
            Jump();

            ConsumeInventory(AbilityType.ExtraJump, _currentValue);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void Dash()
    {
        _currentValue = inventory.GetValueOrDefault(AbilityType.Dash);
        if (_currentValue > 0 && !_isDashing)
        {
            _isDashing = true;
            _ySpeedLimit = maxYSpeed;
            
            Vector3 direction = GetDirection(cameraTransform);
            Vector3 forceToApply = direction * dashForce;

            _rb.useGravity = false;
            _delayedForce = forceToApply;

            Invoke(nameof(DelayedDashForce), 0.025f);
            ConsumeInventory(AbilityType.Dash, _currentValue);
            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    private Vector3 GetDirection(Transform forwardTransform)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = forwardTransform.forward * verticalInput + forwardTransform.right * horizontalInput;

        if (verticalInput == 0 && horizontalInput == 0)
        {
            direction = forwardTransform.forward;
        }

        return direction.normalized;
    }

    private void DelayedDashForce()
    {
        _rb.velocity = Vector3.zero;
        _rb.AddForce(_delayedForce, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        _isDashing = false;
        _rb.useGravity = true;
        _ySpeedLimit = 0;
    }

    private void Stomp()
    {
        _currentValue = inventory.GetValueOrDefault(AbilityType.Stomp);
        if (_currentValue > 0)
        {
            // Stomp
            ConsumeInventory(AbilityType.Stomp, _currentValue);
        }
    }

    public void ConsumeInventory(AbilityType type, int value)
    {
        value -= 1;
        inventory[type] = value;
        if (value == 0)
        {
            bool found = false;

            for (int i = 1; i <= inventory.Count - 1; i++)
            {
                if (inventory.GetValueOrDefault((AbilityType)i) > 0)
                {
                    currentAbility = (AbilityType)i;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                currentAbility = AbilityType.Base;
            }
        }
    }
}
