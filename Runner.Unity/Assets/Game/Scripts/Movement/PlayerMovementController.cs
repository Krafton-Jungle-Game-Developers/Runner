using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType { Base, ExtraJump, Dash, Stomp }
public enum MovementState { Running, Dashing, Air }

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    private Rigidbody _rigidbody;

    private MovementState state;
    private MovementState lastState;

    private float _playerHeight;
    private bool _isGrounded;
    private bool _keepMomentum;

    [Space][Header("Movement")]
    [SerializeField] private float runForce;
    [SerializeField] private float maxYSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float gravity;
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
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
        _playerHeight = GetComponent<CapsuleCollider>().height;
    }

    private void Update()
    {
        CheckGrounded();
        StateHandler();
        MyInput();
        SpeedControl();

        if(state == MovementState.Running)
        {
            _rigidbody.drag = groundDrag;
        }
        else
        {
            _rigidbody.drag = 0;
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
        float distance = (_playerHeight * 0.5f) + 0.2f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            _isGrounded = true;
        }
        else
        {
            // 0.2초 동안 점프 누를수 있게?
            _isGrounded = false;
        }
    }

    private void MyInput()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && _canJump && _isGrounded)
        {
            _canJump = false;

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
            _desiredMoveSpeed = runForce;
        }

        else if (!_isGrounded)
        {
            state = MovementState.Air;
            _desiredMoveSpeed = runForce;
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
/*        Vector3 _moveInput = new Vector3(_xInput, _yInput, 0);
        _moveDirection = transform.forward * _yInput + transform.right * _xInput;
        Vector3 _targetSpeed = _moveInput * _moveSpeed;
        Vector3 speedDiff = _targetSpeed - _rigidbody.velocity;
        Vector3 movement = Mathf.Pow(Mathf.Abs(speedDiff) * _targetSpeed, )

        _rigidbody.AddForce(movement, ForceMode.Force);
*/
        _moveDirection = transform.forward * _yInput + transform.right * _xInput;
        _rigidbody.AddForce(Physics.gravity * (gravity - 1) * _rigidbody.mass);

        if (_isDashing)
        {
            _moveSpeed = dashForce;
        }
        // on ground
        if(_isGrounded)
        {
            _rigidbody.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
        }
        // in air
        else if(!_isGrounded)
        {
            _rigidbody.AddForce(_moveDirection.normalized * _moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 _flatVelocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);

        if(_flatVelocity.magnitude > _moveSpeed)
        {
            Vector3 _limitedVelocity = _flatVelocity.normalized * _moveSpeed;
            _rigidbody.velocity = new Vector3(_limitedVelocity.x, _rigidbody.velocity.y, _limitedVelocity.z);
        }

        if (_ySpeedLimit != 0 && _rigidbody.velocity.y > _ySpeedLimit)
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _ySpeedLimit, _rigidbody.velocity.z);
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
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        _rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
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

            _rigidbody.useGravity = false;
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
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.AddForce(_delayedForce, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        _isDashing = false;
        _rigidbody.useGravity = true;
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
