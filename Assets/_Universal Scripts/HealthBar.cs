using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public float currentHealth; //
    public float maxHealth;
    public TextMeshProUGUI healthNumbers;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        if (healthNumbers != null) //option to show numbers
            maxHealth = slider.maxValue;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        if (healthNumbers != null) 
            healthNumbers.text = slider.value.ToString() + " / " + maxHealth.ToString();
    }
}