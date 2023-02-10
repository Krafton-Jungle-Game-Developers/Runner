using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public AbilityType type;
    private int _currentValue;
    private AbilityType _currentAbility;
    private PlayerController playerController;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
        if (_currentAbility == AbilityType.Base)
        {
            playerController.currentAbility = type;
        }
        _currentValue += 1;
        playerController.inventory[type] = _currentValue;
    }
}
