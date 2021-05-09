using UnityEngine;
using Cinemachine;

public class ScreenShakeListener : MonoBehaviour
{
    [Header("*This uses Cinemachine, camera bounds will affect the screenshake.")]
    [Header("To use this, add a Screenshake Unity Event -> Screenshake Listener -> Shake()")]
    public string placeholderVariable = "";

    private CinemachineImpulseSource source;

    private void Awake()
    {
        source = GetComponent<CinemachineImpulseSource>();
    }

    void Start()
    {
        InvokeRepeating("Shake", 3f, 4f);
    }

    public void Shake()
    {
        source.GenerateImpulse();
    }
}
