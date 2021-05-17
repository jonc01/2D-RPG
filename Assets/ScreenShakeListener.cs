using UnityEngine;
using Cinemachine;

public class ScreenShakeListener : MonoBehaviour
{
    [Header("*This uses Cinemachine, camera bounds will affect the screenshake.")]
    public string placeholderVariable = "";

    private CinemachineImpulseSource source;

    private void Awake()
    {
        source = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake()
    {
        source.GenerateImpulse();
    }
}
