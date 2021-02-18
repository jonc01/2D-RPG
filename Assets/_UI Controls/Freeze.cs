using System.Collections;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    bool isFrozen = false;
    public float freezeDuration = 0.1f;

    private void Update()
    {
        /*if (Input.GetKeyDown("u"))
        {
            StartFreeze();
        }*/
        //Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime; // slowly increases scale until it is back to 1.0
        //Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }

    public void StartFreeze()
    {
        if (!isFrozen)
            StartCoroutine(FreezeTime(freezeDuration));
    }

    IEnumerator FreezeTime(float freezeDuration)
    {
        Time.timeScale = 0f;
        isFrozen = true;
        yield return new WaitForSeconds(freezeDuration);
        Time.timeScale = 1f;
        isFrozen = false;
    }
}
