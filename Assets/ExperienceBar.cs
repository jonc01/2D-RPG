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
        200, //2
        300
        }; 

    public int currentPlayerLevel;
    //public float XPmultiplier = 1.0f;

    //might not need, just use slider values, calculate new values on LevelUp() call
    public int currentXP; //displayed in slider.value
    public int maxXP; //total XP needed to level up

    public void SetXP(int currentXP, int currentLevel) //call after level up
    {
        slider.maxValue = totalLevelXP[currentLevel - 1];
        slider.value = currentXP;
    }

    private void Update()
    {
        if (Input.GetKeyDown("i")) //TESTING
        {
            Debug.Log("give 10 XP");
            AddXP(11);
        }
    }

    public void AddXP(float xp)
    {
        slider.value += xp;

        if(xp > 0)
        {
            //XP instantiate above player or player health bar
        }
        
        if (slider.value >= slider.maxValue) //Level Up; currentXP has met/exceeded XP needed
        {
            float overflowXP = (slider.value - slider.maxValue);
            if(overflowXP > 0) //exceeded XP for level
            {
                Debug.Log("Overflow XP: " + overflowXP);
                LevelUp(overflowXP);
            }
            else
            {
                Debug.Log("No overflow XP");
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