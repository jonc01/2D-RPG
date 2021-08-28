using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryBufferController : MonoBehaviour
{
    [Header("=== References Required for Setup ===")]
    public StationaryBufferClass bufferClass;
    public StationaryBufferRaycast bufferRaycast;

    // Start is called before the first frame update
    void Start()
    {
        //should override, not moving, etc
    }

    // Update is called once per frame
    void Update()
    {
        BuffCheck();
    }

    void BuffCheck()
    {
        if (bufferRaycast.friendlyDetectBack || bufferRaycast.friendlyDetectFront)
            bufferClass.StartBuffing();

    }
}
