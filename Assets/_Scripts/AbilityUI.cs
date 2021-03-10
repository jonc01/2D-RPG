using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUI : MonoBehaviour
{
    //PlayerCombat controls inputs and sends cooldown values
    public Image abilityIcon;
    [SerializeField] float abilityCooldown;
    [SerializeField] bool coolingDown = false;

    //Timer
    public CountdownTimer countdownTimer;

    void Update()
    {
        Cooldown();
        CheckCooldown();
    }

    public void StartCooldown(float abilityCD) //called when ability is used
    {
        abilityCooldown = abilityCD;
        abilityIcon.fillAmount = 0;
        coolingDown = true;
        if(countdownTimer)
            countdownTimer.StartCountdown(abilityCD);
    }

    void Cooldown()
    {
        if(coolingDown == true)
            abilityIcon.fillAmount += 1.0f / abilityCooldown * Time.deltaTime;
    }

    void CheckCooldown()
    {
        if(abilityIcon.fillAmount >= 1)
            coolingDown = false;
    }
}
