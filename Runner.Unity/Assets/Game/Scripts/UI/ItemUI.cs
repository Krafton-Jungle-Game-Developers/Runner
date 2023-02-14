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
    [SerializeField] private Color JumpColor = Color.green;
    [SerializeField] private Color DashColor = Color.blue;
    [SerializeField] private Color StompColor = Color.yellow;

    private AbilityType _frontType = AbilityType.Base;
    private int _numFront = 0;
    private AbilityType _lastType = AbilityType.Base;
    private int _numLast = 0;

/*    [SerializeField] private GameObject BaseCardPrefab;
    [SerializeField] private GameObject JumpCardPrefab;
    [SerializeField] private GameObject DashCardPrefab;
    [SerializeField] private GameObject StompCardPrefab;*/

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
        ItemTextFront.text = _frontType.ToString() + _numFront.ToString();
        
        ItemTextLast.text = _lastType.ToString() + _numLast.ToString();
        
    }

    

    public void ItemCounterUpdate()
    {
        _frontType = playerController.currentAbility;
        _numFront= playerController.inventory[_frontType];
        ColorChange(_frontType, ItemTextFront);

        _lastType = playerController.secondaryAbility;
        _numLast = playerController.inventory[_lastType];
        ColorChange(_lastType, ItemTextLast);

        ItemTextFront.text = _frontType.ToString() + " " + _numFront.ToString();
        ItemTextLast.text = _lastType.ToString() + " " + _numLast.ToString();
    }

    private void ColorChange(AbilityType abilityType, TMP_Text text)
    {
        switch(abilityType)
        {
            case AbilityType.Base:
                text.color = Color.grey;
                break;
            case AbilityType.AirJump: 
                text.color = JumpColor;
                break;
            case AbilityType.Dash:
                text.color = DashColor;
                break;   
            case AbilityType.Stomp:
                text.color = StompColor;
                break;
        }
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
