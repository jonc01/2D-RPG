using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthPotion : MonoBehaviour
{
    //Consumables script, keeps track of 

    public PlayerCombat playerCombat;
    public TextMeshProUGUI numPotionCharges;

    //slider if using regenerating charges based on kills

    int currentPotionCharges;
    int defaultAmountPotions = 3; //placeholder until inventory is implemented
    int maxPotions = 10;

    public Slider chargeSlider; //killing enemies adds progress to charge bar
    int maxSliderValue = 50;

    void Start()
    {
        currentPotionCharges = defaultAmountPotions;
        //currentPotionCharges = playerCombat.potionsOwned;
        UpdatePotionDisplay();
        chargeSlider.maxValue = maxSliderValue;
        chargeSlider.value = 0;
    }

    private void LateUpdate()
    {
        UpdatePotionDisplay();
    }

    void UpdatePotionDisplay()
    {
        numPotionCharges.text = currentPotionCharges.ToString() + "/" + maxPotions.ToString();
    }

    public void AddPotionCharge(int numCharges = 1)
    {
        currentPotionCharges += numCharges;
        if(currentPotionCharges > maxPotions)
        {
            currentPotionCharges = maxPotions;
        }
    }

    public void GetChargeFromKill(float chargeAmount = 10)
    {
        chargeSlider.value += chargeAmount;
        if(chargeSlider.value == maxSliderValue)
        {
            AddPotionCharge();
            chargeSlider.value = 0;
        }
        else if(chargeSlider.value > maxSliderValue) //overflow to next charge
        {
            float temp = chargeSlider.value - maxSliderValue;
            chargeSlider.value = temp;
            AddPotionCharge();
        }
    }

    public void UsePotionCharge()
    {
        if(playerCombat.isAlive && 
            playerCombat.currentHealth < playerCombat.maxHealth && 
            currentPotionCharges > 0)
        {
            playerCombat.HealPlayer(25);
            currentPotionCharges--;
        }
    }
}
