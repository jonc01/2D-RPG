using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    public Slider slider; //Experience Bar

    //Level 1 = [0], etc
    float[] totalLevelXP = 
        {
        100, //1
        500, //2
        1000
        }; 

    public int currentPlayerLevel;
    //public float XPmultiplier = 1.0f;

    //might not need, just use slider values, calculate new values on LevelUp() call
    public int currentXP; //displayed in slider.value
    public int maxXP; //total XP needed to level up

    public void SetXP(int xp, int currentLevel) //call after level up
    {
        slider.maxValue = xp;
        slider.value = xp;
    }

    public void AddXP(float xp)
    {
        slider.value += xp;

        if(xp > 0)
        {
            //XP instantiate above player or player health bar
        }
        
        if (slider.value >= slider.maxValue) //currentXP has met/exceeded XP needed to level up
        {
            float overflowXP = (slider.value - slider.maxValue);
            if(overflowXP > 0) //exceeded XP for level
            {
                LevelUp(overflowXP);
            }
            else
            {
                LevelUp();
            }
        }
    }

    void LevelUp(float overflowXP = 0)
    {
        currentPlayerLevel++;
        slider.value = 0;
        slider.maxValue = totalLevelXP[currentPlayerLevel - 1]; //PLACEHOLDER

        if(overflowXP > 0)
            AddXP(overflowXP);
    }
}