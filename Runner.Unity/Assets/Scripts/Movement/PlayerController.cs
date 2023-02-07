using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType { ExtraJump, Dash, Stomp }

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    Vector3 velocity;

    [Header("Ability")]
    public KeyCode abilityKey = KeyCode.Mouse1;
    public InventoryDictionary<AbilityType, int> inventory = new InventoryDictionary<AbilityType, int>();
    private int currentValue;
    public AbilityType currentAbility;

    [Header("Jump")]
    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower;

    [Header("Dash")]
    public float dashPower;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inventory.Add(AbilityType.ExtraJump, 0);
        inventory.Add(AbilityType.Dash, 0);
        inventory.Add(AbilityType.Stomp, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(abilityKey))
        {
            UseAbility();
        }
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

    private void AirJump()
    {
        currentValue = inventory.GetValueOrDefault(AbilityType.ExtraJump);
        if (currentValue > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);

            currentValue -= 1;
            inventory[AbilityType.ExtraJump] = currentValue;
        }
    }

    private void Dash()
    {
        currentValue = inventory.GetValueOrDefault(AbilityType.Dash);
        if (currentValue > 0)
        {
            rb.AddForce(rb.velocity.normalized * dashPower, ForceMode.Impulse);
            currentValue -= 1;
            inventory[AbilityType.Dash] = currentValue;
        }
    }

    private void Stomp()
    {
        currentValue = inventory.GetValueOrDefault(AbilityType.Stomp);
        if (currentValue > 0)
        {
            // Stomp
            currentValue -= 1;
            inventory[AbilityType.Stomp] = currentValue;
        }
    }
    public void ConsumeInventory()
    {

    }
}
