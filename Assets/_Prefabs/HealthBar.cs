using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider slider; //main health bar
    public Slider trail; //trails behind main slider to show amount of damage taken in short period
    public float currentHealth; //
    public float maxHealth;
    public TextMeshProUGUI healthNumbers;

    private float trailDelayTimer = .5f; //delay after taking damage to start trailing
    private float trailDiff; //difference of trail value and main health bar value
    private bool damageTakenRecent = false;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        if (healthNumbers != null) //option to show numbers
            maxHealth = slider.maxValue;

        if (trail != null)
            trail.maxValue = health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        if (healthNumbers != null) //adding health numbers
            healthNumbers.text = slider.value.ToString() + " / " + maxHealth.ToString();

        if (trail != null)
        {
            if (slider.value > trail.value)
            trail.value = slider.value;

            if (damageTakenRecent == false)
                Invoke("DamageTaken", .1f);
                //DamageTaken();
                //StartCoroutine(DamageTakenDelay());
        }
    }

    void LateUpdate()
    {
        if (trail != null)
        {
            if (trail.value > slider.value)
            {
                trailDiff = trail.value - slider.value;
                if (damageTakenRecent == false)
                    StartCoroutine(TrailHealth());
            }
        }
    }
    
    void DamageTaken()
    {
        StartCoroutine(DamageTakenDelay());
    }

    IEnumerator DamageTakenDelay()
    {
        damageTakenRecent = true;
        yield return new WaitForSeconds(trailDelayTimer); //if no damage is taken within this time, start trail
        damageTakenRecent = false;
    }

    IEnumerator TrailHealth()
    {
        while(trail.value > slider.value)
        {
            for(int i=0; i<trailDiff; i++)
            {
                trail.value--;
                yield return new WaitForSeconds(.5f); //speed of trailing
                if(damageTakenRecent == true)
                {
                    yield break; //damage was taken, break out of loop/coroutine
                }
            }
            yield break; //break out of coroutine if loop ends
        }
    }
}