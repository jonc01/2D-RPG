using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    public Transform StartWall;
    public Transform EndWall;
    public float minCamPos;
    public float maxCamPos;
    public float boundsOffset = 5.59f;

    //keep camera inbounds

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        minCamPos = StartWall.position.x + boundsOffset;
        maxCamPos = EndWall.position.x - boundsOffset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerTransform == null)
            return;
        
        /*Vector3 temp = transform.position;
        temp.x = playerTransform.position.x;
        transform.position = temp;*/

        Vector3 temp = transform.position + offset;
        temp.x = playerTransform.position.x;

        //if(temp.x > minCamPos && temp.x < maxCamPos) //camera is over [offset] from Start wall, camera is under [offset] from End wall
        
        Vector3 smoothPosition = Vector3.Lerp(transform.position, temp, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }
}
