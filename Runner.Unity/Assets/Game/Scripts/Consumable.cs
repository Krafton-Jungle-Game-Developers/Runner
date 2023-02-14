using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public AbilityType type;
    private int _currentValue;
    private AbilityType _currentAbility;
    private PlayerMovementController playerController;
    private ItemUI itemCounter;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
        itemCounter = GameObject.FindObjectOfType<ItemUI>();
    }

    private void Update()
    {
        // animation?
    }

    private void OnTriggerEnter(Collider other)
    {
        AddToInventory(type);
        Destroy(gameObject);
    }
    private void AddToInventory(AbilityType type)
    {
        _currentValue = playerController.inventory.GetValueOrDefault(type);
        _currentAbility = playerController.currentAbility;

        if (playerController.currentAbility != AbilityType.Base && playerController.secondaryAbility != AbilityType.Base)
        {
            playerController.inventory[playerController.secondaryAbility] = 0;
            playerController.secondaryAbility = playerController.currentAbility;
        }
        else if (playerController.currentAbility != AbilityType.Base && playerController.secondaryAbility == AbilityType.Base)
        {
            playerController.secondaryAbility = playerController.currentAbility;
        }
        playerController.currentAbility = type;
        _currentValue += 1;
        playerController.inventory[type] = _currentValue;

        itemCounter.ItemCounterUpdate();
    }
}
