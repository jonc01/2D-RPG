using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu Instance { get; private set; }

    public GameObject screenshakeON;
    public GameObject screenshakeOFF;

    private void Awake()
    {
        Instance = this;
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        // Screenshake
        if (ScreenShakeListener.enableScreenshake)
        {
            //update buttons
            if(screenshakeON != null) screenshakeON.SetActive(true);
            if(screenshakeOFF != null) screenshakeOFF.SetActive(false);
        }
        else
        {
            if(screenshakeON != null) screenshakeON.SetActive(false);
            if(screenshakeOFF != null) screenshakeOFF.SetActive(true);
        }
    }

    public void ToggleScreenShake()
    {
        if (ScreenShakeListener.enableScreenshake)
        {
            ScreenShakeListener.enableScreenshake = false;
        }
        else
        {
            ScreenShakeListener.enableScreenshake = true;
        }
        UpdateSettings();
    }
}
