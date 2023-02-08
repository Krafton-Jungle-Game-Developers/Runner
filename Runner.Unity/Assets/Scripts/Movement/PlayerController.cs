using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType { ExtraJump, Dash, Stomp }

public class PlayerController : MonoBehaviour
{
    public Transform orientation;
    private float playerHeight;
    private Rigidbody _rb;
    private bool _isGrounded;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float airMultiplier;
    public float gravity;
    private float _xInput;
    private float _yInput;
    private Vector3 _moveDirection;

    [Header("Ability")]
    public KeyCode abilityKey;
    public AbilityType currentAbility;
    public InventoryDictionary<AbilityType, int> inventory = new InventoryDictionary<AbilityType, int>();
    private int _currentValue;

    [Header("Jump")]
    public KeyCode jumpKey;
    public float jumpPower;
    public float jumpCooldown;
    private bool _canJump = true;

    [Header("Dash")]
    public float dashPower;
    private bool _isDashing = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        playerHeight = gameObject.GetComponentInChildren<CapsuleCollider>().height;
        _rb.freezeRotation = true;
        inventory.Add(AbilityType.ExtraJump, 0);
        inventory.Add(AbilityType.Dash, 0);
        inventory.Add(AbilityType.Stomp, 0);
    }

    void Update()
    {
        CheckGrounded();
        MyInput();
        SpeedControl();

        if(_isGrounded)
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

    private void MovePlayer()
    {
        _moveDirection = orientation.forward * _yInput + orientation.right * _xInput;
        _rb.AddForce(Physics.gravity * (gravity - 1) * _rb.mass);

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
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _canJump = true;
    }

    public void UseAbility()
    {
        switch (currentAbility)
        {
        case AbilityType.ExtraJump:
            AirJump();
            break;

        case AbilityType.Dash:
            if (!_isDashing)
            {
                Dash();
            }
            break;

        case AbilityType.Stomp:
            Stomp();
            break;
        }
    }

    private void AirJump()
    {
        _currentValue = inventory.GetValueOrDefault(AbilityType.ExtraJump);
        if (_currentValue > 0 && _canJump && !_isGrounded)
        {
            _canJump = false;

            Jump();

            _currentValue -= 1;
            inventory[AbilityType.ExtraJump] = _currentValue;

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void Dash()
    {
        _currentValue = inventory.GetValueOrDefault(AbilityType.Dash);
        if (_currentValue > 0)
        {
/*            GetComponent<>
            Vector3 lookDirection = new Vector3 ()*/
            _rb.AddForce(_rb.velocity.normalized * dashPower, ForceMode.Impulse);
            _currentValue -= 1;
            inventory[AbilityType.Dash] = _currentValue;
        }
    }

    private void Stomp()
    {
        _currentValue = inventory.GetValueOrDefault(AbilityType.Stomp);
        if (_currentValue > 0)
        {
            // Stomp
            _currentValue -= 1;
            inventory[AbilityType.Stomp] = _currentValue;
        }
    }

    public void ConsumeInventory()
    {

    }
}
