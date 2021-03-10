using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public int cdTime;
    public TextMeshProUGUI cdDisplay;

    public void StartCountdown(float cooldownTime)
    {
        cdDisplay.GetComponent<TextMeshProUGUI>().enabled = true;
        //cdDisplay.gameObject.SetActive(true); //display timer text
        cdTime = (int)cooldownTime;
        StartCoroutine(CountdownTime());
    }

    IEnumerator CountdownTime()
    {
        while(cdTime > 0)
        {
            cdDisplay.text = cdTime.ToString();
            yield return new WaitForSeconds(1f);
            cdTime--;
        }
        cdDisplay.GetComponent<TextMeshProUGUI>().enabled = false;
        //cdDisplay.gameObject.SetActive(false); //timer hits 0
    }
}
