using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    //slow down time
    public float slowdownFactor = 0.02f;
    public float slowdownLength = 2f;
    bool isSlowed = false;

    //[Space] //freeze time
    //public float freezeLength = .1f;
    //bool isFrozen = false;

    private void Update()
    {
        //Slow Time
        if (Time.timeScale == 1f && isSlowed)
        {
            isSlowed = false; //
        }

        if (Time.timeScale < 1f && isSlowed) //only if Slow Time is called, gradually return back to normal speed
        {
            Time.timeScale += (1f/slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f); //gradually increases scale until it is back to 1.0

            Time.fixedDeltaTime = Mathf.Clamp(Time.timeScale, 0.0004f, .02f); //player move speed uses fixedDeltaTime, need to reset here
        }
    }

    public void DoSlowMotion()
    {
        isSlowed = true;
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    public void DoFreezeTime(float duration = .1f)
    {
        StartCoroutine(FreezeTime(duration));
    }

    IEnumerator FreezeTime(float freezeDuration)
    {
        //isFrozen = true;
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(freezeDuration);

        //isFrozen = false;
        Time.timeScale = 1; //set timeScale back to default scale
    }
}
