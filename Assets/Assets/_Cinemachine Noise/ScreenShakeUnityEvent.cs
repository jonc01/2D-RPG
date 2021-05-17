using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class ScreenShakeUnityEvent : MonoBehaviour
{
    [Header("Test event shakes screen every 4 seconds")]
    public UnityEvent screenShake;
    void Start()
    {
        InvokeRepeating("ScreenShakeEvent", 3f, 4f);
    }

    private void ScreenShakeEvent()
    {
        screenShake.Invoke();
    }
}
