using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    //PlayerCombat controls inputs and sends cooldown values here
    public Image abilityIcon;
    [SerializeField] float abilityCooldown;
    [SerializeField] bool coolingDown = false;

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
