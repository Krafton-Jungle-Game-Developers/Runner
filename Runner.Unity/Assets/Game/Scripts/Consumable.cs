using System;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public AbilityType type;
    private PlayerMovementController playerController;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
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
        AbilityType currentAbility = playerController.currentAbility;
        AbilityType secondaryAbility = playerController.secondaryAbility;
        int value = playerController.inventory.GetValueOrDefault(type);

        if (currentAbility == type)
        {
            value += 1;
            playerController.inventory[type] = value;
        }
        else if (secondaryAbility == type)
        {
            value += 1;
            playerController.inventory[type] = value;
            playerController.SwapInventory();
        }
        else
        {
            if (playerController.currentAbility != AbilityType.Base && playerController.secondaryAbility != AbilityType.Base)
            {
                playerController.inventory[secondaryAbility] = 0;
                playerController.secondaryAbility = currentAbility;
            }
            else if (playerController.currentAbility != AbilityType.Base && playerController.secondaryAbility == AbilityType.Base)
            {
                playerController.secondaryAbility = currentAbility;
            }
            playerController.currentAbility = type;
            value += 1;
            playerController.inventory[type] = value;
        }
    }
}
