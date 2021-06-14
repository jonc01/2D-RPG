using UnityEngine;
using Cinemachine;

public class ScreenShakeListener : MonoBehaviour
{
    [Header("*This uses Cinemachine, camera bounds will affect the screenshake.")]
    public string placeholderVariable = "";

    public static bool enableScreenshake = true;
    //Sources listed in order of impulse strength
    public CinemachineImpulseSource source1;
    public CinemachineImpulseSource source2;
    public CinemachineImpulseSource source3;

    public void Shake(int sourceChoice = 1)
    {
        if (enableScreenshake && source1 != null)
        {
            switch (sourceChoice)
            {
                case 1:
                    source1.GenerateImpulse();
                    break;
                case 2:
                    source2.GenerateImpulse();
                    break;
                case 3:
                    source3.GenerateImpulse();
                    break;
                default:
                    //Debug.Log("No impulse source.");
                    break;
            }
        }
    }
}
