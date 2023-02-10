using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType { Base, ExtraJump, Dash, Stomp }
public enum MovementState { Running, Dashing, Air }

public class PlayerController : MonoBehaviour
{
    public Transform orientation;
    public Transform playerCam;
    private MovementState state;
    private MovementState lastState;
    private float playerHeight;
    private Rigidbody _rb;
    private bool _isGrounded;
    private bool keepMomentum;

    [Header("Movement")]
    public float runForce;
    public float maxYSpeed;
    public float groundDrag;
    public float airMultiplier;
    public float gravity;
    private float ySpeedLimit;
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private float _xInput;
    private float _yInput;
    private Vector3 _moveDirection;

    [Header("Ability")]
    public KeyCode abilityKey;
    public AbilityType currentAbility = AbilityType.Base;
    public InventoryDictionary<AbilityType, int> inventory = new InventoryDictionary<AbilityType, int>();
    private int _currentValue;

    [Header("Jump")]
    public KeyCode jumpKey;
    public float jumpForce;
    public float jumpCooldown;
    private bool _canJump = true;

    [Header("Dash")]
    public float dashForce;
    public float dashSpeedChangeFactor;
    private float speedChangeFactor;
    private Vector3 _delayedForce;
    public float dashCooldown;
    private bool _isDashing = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        playerHeight = gameObject.GetComponentInChildren<CapsuleCollider>().height;
        _rb.freezeRotation = true;
        inventory.Add(AbilityType.Base, 0);
        inventory.Add(AbilityType.ExtraJump, 0);
        inventory.Add(AbilityType.Dash, 0);
        inventory.Add(AbilityType.Stomp, 0);
    }

    private void Update()
    {
        CheckGrounded();
        StateHandler();
        MyInput();
        SpeedControl();

        if(state == MovementState.Running)
        {
            _rb.drag = groundDrag;
        }
        else
        {
            _rb.drag = 0;
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
        float distance = (playerHeight * 0.5f) + 0.2f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
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

        if(Input.GetKeyDown(jumpKey) && _canJump && _isGrounded)
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
            desiredMoveSpeed = dashForce;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        else if (_isGrounded)
        {
            state = MovementState.Running;
            desiredMoveSpeed = runForce;
        }

        else if (!_isGrounded)
        {
            state = MovementState.Air;
            desiredMoveSpeed = runForce;
        }

        bool desiredMoveSpeedChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.Dashing)
        {
            keepMomentum = true;
        }
        if (desiredMoveSpeedChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(LerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }
    private void MovePlayer()
    {
        _moveDirection = orientation.forward * _yInput + orientation.right * _xInput;
        _rb.AddForce(Physics.gravity * (gravity - 1) * _rb.mass);

        if (_isDashing)
        {
            moveSpeed = dashForce;
        }
        // on ground
        if(_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        // in air
        else if(!_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 _flatVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        if(_flatVelocity.magnitude > moveSpeed)
        {
            Vector3 _limitedVelocity = _flatVelocity.normalized * moveSpeed;
            _rb.velocity = new Vector3(_limitedVelocity.x, _rb.velocity.y, _limitedVelocity.z);
        }

        if (ySpeedLimit != 0 && _rb.velocity.y > ySpeedLimit)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, ySpeedLimit, _rb.velocity.z);
        }
    }

    private IEnumerator LerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
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
            ySpeedLimit = maxYSpeed;
            Transform forwardTransform = playerCam;
            Vector3 direction = GetDirection(forwardTransform);
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
        ySpeedLimit = 0;
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
