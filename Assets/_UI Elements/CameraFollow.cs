using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTransform;

    //keep camera inbounds
    


    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        //
    }


    // Update is called once per frame
    void LateUpdate()
    {
        if (playerTransform == null)
            return;

        Vector3 temp = transform.position;
        //camera moved to player position
        temp.x = playerTransform.position.x;
        transform.position = temp;
        //
        /*temp.y = playerTransform.position.y;
        transform.position = temp;*/
    }
}
