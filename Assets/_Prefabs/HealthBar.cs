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

    private float trailDelay = 1.0f; //delay after taking damage to start trailing
    private bool damageTakenRecent = false;
    private float trailDelayTimer = 0.0f;
    private float trailProp;
    private float trailRate = 1.0f; //lower values = faster trail, higher values = slower trail

    public bool slowTrail = false;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;

        if (slowTrail)
        {
            trailProp = 6f / trail.maxValue; //this value is the delay of the coroutine reducing the trail slider value
        }
        else
        {
            trailProp = trailRate / trail.maxValue; //trail will move at rate proportional to max hp of object
        }
        
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
        }
    }

    void LateUpdate()
    {
        if (trail != null)
        {
            if (trail.value > slider.value)
            {
                //trailDiff = trail.value - slider.value; //to show individual sections of damage
                if(Time.time > trailDelayTimer && damageTakenRecent == false)
                {
                    StartCoroutine(TrailHealth());
                }
            }
        }
    }

    IEnumerator TrailHealth()
    {
        damageTakenRecent = true; //prevent coroutine being called multiple times
        trailDelayTimer = Time.time + trailDelay;

        while (trail.value > slider.value)
        {
            trail.value--; //keep decrement at 1 for smooth trail, only increase trail rate
            yield return new WaitForSeconds(trailProp); //speed of trailing
        }
        damageTakenRecent = false;
        yield break; //break out of coroutine when loop ends
    }
}