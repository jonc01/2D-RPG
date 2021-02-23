using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceBar : MonoBehaviour
{
    public Slider slider; //Experience Bar
    public TextMeshProUGUI displayPlayerLevel;
    public HealthBar healthBar;
    public PlayerCombat playerCombat;
    public TextPopupsHandler textPopupsHandler;
    public TextPopupsHandler xpPopups;
    public Transform player;

    //float[] totalLevelXP = {100, 200, 300}; 

    public float currentPlayerLevel; 
    //public float XPmultiplier = 1.0f;

    //might not need, just use slider values, calculate new values on LevelUp() call
    public int currentXP; //displayed in slider.value
    public int maxXP; //total XP needed to level up

    public void SetXP(int currentXP, float currentLevel) //call after level up
    {
        //slider.maxValue = totalLevelXP[currentLevel - 1];
        slider.maxValue = (currentPlayerLevel * 100) * 1.25f;
        slider.value = currentXP;

        if (displayPlayerLevel != null) //display level
            displayPlayerLevel.text = currentLevel.ToString();
    }

    public void AddXP(float xp)
    {
        float overflowXP = 0;
        float xpNeeded = slider.maxValue - slider.value;
        slider.value += xp;

        if (xp > xpNeeded)
            overflowXP = xp - xpNeeded;

        if(xp > 0)
        {
            //XP instantiate above player
        }

        if(slider.value >= slider.maxValue) //XP requirement met
            LevelUp(overflowXP);
    }

    void LevelUp(float overflowXP = 0)
    {
        currentPlayerLevel++;
        slider.value = overflowXP; //reset progress and add overflowXP if xp exceeded level up
        slider.maxValue = (currentPlayerLevel * 100) * 1.25f;
        if (displayPlayerLevel != null) //update level
            displayPlayerLevel.text = currentPlayerLevel.ToString();

        healthBar.SetMaxHealth(100 + ((currentPlayerLevel - 1) * 10f));
        playerCombat.HealPlayer(healthBar.maxHealth);

        xpPopups.ShowText(player.position, "\nLEVEL UP");
    }
}