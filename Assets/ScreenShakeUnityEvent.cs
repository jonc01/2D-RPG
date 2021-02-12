using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class ScreenShakeUnityEvent : MonoBehaviour
{
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
