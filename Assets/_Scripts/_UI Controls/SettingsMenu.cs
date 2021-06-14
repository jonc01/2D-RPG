using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject screenshakeON;
    public GameObject screenshakeOFF;


    public void UpdateSettings()
    {
        // Screenshake
        if (ScreenShakeListener.enableScreenshake)
        {
            //update buttons
            screenshakeON.SetActive(true);
            screenshakeOFF.SetActive(false);
        }
        else
        {
            screenshakeON.SetActive(false);
            screenshakeOFF.SetActive(true);
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
