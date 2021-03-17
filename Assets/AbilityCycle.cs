using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCycle : MonoBehaviour
{
    public Image[] abilities; //input number of abilities to cycle through

    public void ShowAbility(int currentAbility, bool enableAbility = true) //default to first image //default true
    {
        abilities[currentAbility].enabled = enableAbility; //display current ablity
        
        for(int i = 0; i < abilities.Length; i++) //disable rest by default
        {
            if(i != currentAbility)
                abilities[i].enabled = !enableAbility;
        }
    }

    public void ShowAbilities(int currentAbility1, int currentAbility2, bool enableAbility = true) //default to first image //default true
    {
        abilities[currentAbility1].enabled = enableAbility; //display current ability
        abilities[currentAbility2].enabled = enableAbility;
        
        for (int i = 0; i < abilities.Length; i++) //disable rest by default
        {
            if (i != currentAbility1 || i != currentAbility2)
                abilities[i].enabled = !enableAbility;
        }
    }
}
