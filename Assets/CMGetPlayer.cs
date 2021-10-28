using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CMGetPlayer : MonoBehaviour
{
    //https://forum.unity.com/threads/cinemachine-target-follow-an-initialize-prefab.559576/

    private CinemachineVirtualCamera vCam;
    public GameObject playerTarget;
    public Transform playerTransform;
    //public GameObject cameraBounds;

    //
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();

        if(playerTarget == null)
        {
            //Look for target object
            playerTarget = GameObject.Find("CameraFocusPoint");
            if(playerTarget != null)
            {
                //If target is found, set variables
                playerTransform = playerTarget.transform;
                vCam.LookAt = playerTransform;
                vCam.Follow = playerTransform;
            }
        }
    }
}
