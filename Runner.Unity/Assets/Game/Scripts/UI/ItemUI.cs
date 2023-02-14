using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

/// <summary>
/// This Scirpt is for Controlling Item Counter UI. 
/// Currently, Implemented on Item UI. 
/// </summary>
public class ItemUI : MonoBehaviour
{
    private PlayerMovementController playerController;
    [SerializeField] private TMP_Text ItemTextFront;
    [SerializeField] private TMP_Text ItemTextLast;
    [SerializeField] private Color frontColor = Color.green;
    [SerializeField] private Color lastColor = Color.gray;

    private AbilityType _frontType = AbilityType.Base;
    private int _numFront = 0;
    private AbilityType _lastType = AbilityType.Base;
    private int _numLast = 0;

    [SerializeField] private GameObject BaseCardPrefab;
    [SerializeField] private GameObject JumpCardPrefab;
    [SerializeField] private GameObject DashCardPrefab;
    [SerializeField] private GameObject StompCardPrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
        ItemTextFront.text = _frontType.ToString() + _numFront.ToString();
        ItemTextFront.color= frontColor;
        ItemTextLast.text = _lastType.ToString() + _numLast.ToString();
        ItemTextLast.color= lastColor;
    }

    

    public void ItemCounterUpdate()
    {
        _frontType = playerController.currentAbility;
        _numFront= playerController.inventory[_frontType];
        //_lastType = playerController.secondAbility;
        //_numLast = playerController.inventory[_lastType];

        ItemTextFront.text = _frontType.ToString() + _numFront.ToString();
        ItemTextLast.text = _lastType.ToString() + _numLast.ToString();
    }

    // Create Card when Inventory update
    /*public void CreateCard(AbilityType abilityType)
    {
        switch (abilityType)
        {
            case AbilityType.Base:
                GameObject newBaseCard = Instantiate(BaseCardPrefab);
                break;
            case AbilityType.ExtraJump:
                GameObject newJumpCard = Instantiate(JumpCardPrefab);
                break;
            case AbilityType.Dash:
                GameObject newDashCard = Instantiate(DashCardPrefab);
                break;
            case AbilityType.Stomp:
                GameObject newStompCard = Instantiate(StompCardPrefab);
                break;
        }
    }*/
}
