using System.Collections;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    public float duration = 1f;
    bool isFrozen = false;
    float _FreezeDuration = 0f;
    void Update()
    {
        if(_FreezeDuration < 0 && !isFrozen)
        {
            StartCoroutine(StartFreeze());
        }
    }

    public void Freezer()
    {
        _FreezeDuration = duration;
    }

    IEnumerator StartFreeze()
    {
        isFrozen = true;
        var original = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = original;
        _FreezeDuration = 0;
        isFrozen = false;
    }
}
