using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float slowdownFactor = 0.02f;
    public float slowdownLength = 2f;

    private void Update()
    {
        Time.timeScale += (1f/slowdownLength) * Time.unscaledDeltaTime; // slowly increases scale until it is back to 1.0
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);

        Time.fixedDeltaTime = Mathf.Clamp(Time.timeScale, 0.0004f, .02f); // player move speed uses fixedDeltaTime, need to manually reset here
    }

    public void DoSlowMotion()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }
}
