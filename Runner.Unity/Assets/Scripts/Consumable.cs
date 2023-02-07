using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public AbilityType type;
    private int currentValue;
    private PlayerController playerController;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        // animation?
    }

    private void OnTriggerEnter(Collider other)
    {
        AddToInventory(type);
        Destroy(gameObject);
    }
    public void AddToInventory(AbilityType type)
    {
        currentValue = playerController.inventory.GetValueOrDefault(type);
        currentValue += 1;
        playerController.inventory[type] = currentValue;
    }
}
