using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    //slow down time
    public float slowdownFactor = 0.02f;
    public float slowdownLength = 2f;
    bool isSlowed = false; //default slowMotion recover behavior
    bool isSlowedCustom = false; //custom times inputted

    //public float freezeLength = 0.1f;

    //[Space] //freeze time
    //public float freezeLength = .1f;
    //bool isFrozen = false;

    private void Update()
    {
        //Slow Time
        if (Time.timeScale == 1f && isSlowed)
        {
            isSlowed = false; //toggle variable after time is back to normal
        }

        if (Time.timeScale < 1f && isSlowed && !PauseMenu.GameIsPaused) //only if Slow Time is called, gradually return back to normal speed
        {
            Time.timeScale += (1f/slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f); //gradually increases scale until it is back to 1.0

            //Time.fixedDeltaTime = Mathf.Clamp(Time.timeScale, 0.0004f, .02f); //player move speed uses fixedDeltaTime, need to reset here
        }
    }

    public void DoSlowMotion()
    {
        //has a gradual return to normal time scale
        isSlowed = true;
        Time.timeScale = slowdownFactor;
        //Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    public void CustomSlowMotion(float slowdownFactorC, float slowdownLengthC)
    {
        //has custom duration for slow motion, instantly returns to normal time scale after
        StartCoroutine(TimedSlowMotion(slowdownFactorC, slowdownLengthC));
    }

    IEnumerator TimedSlowMotion(float slowdownFactor, float slowdownTimer)
    {
        Time.timeScale = slowdownFactor;
        yield return new WaitForSeconds(slowdownTimer);
        ResetTimeScale();
    }

    public void DoFreezeTime(float duration = 0.05f, float delayStart = 0f)
    {
        StartCoroutine(FreezeTime(duration, delayStart));
    }

    IEnumerator FreezeTime(float freezeDuration, float delayStart)
    {
        if(delayStart > 0f)
            yield return new WaitForSeconds(delayStart);

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(freezeDuration); //be careful this actually finishes or else timeScale is stuck at 0

        ResetTimeScale();
    }

    public void ResetTimeScale()
    {
        if (!PauseMenu.GameIsPaused)
        {
            Time.timeScale = 1f; //set timeScale back to default scale
        }
    }
}
