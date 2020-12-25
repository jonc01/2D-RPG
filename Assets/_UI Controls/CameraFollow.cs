using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    //keep camera inbounds

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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

        Vector3 smoothPosition = Vector3.Lerp(transform.position, temp, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }
}
