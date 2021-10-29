using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryBufferRaycast : MonoBehaviour
{
    [Header("=== Required reference setup ===")]
    [SerializeField] private bool debugging;
    public LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;

    [Header("=== Adjustable Variables ===")]
    [SerializeField] private float buffRange;

    [Header("=== Raycast Checks ===")]
    public bool friendlyDetectFront, friendlyDetectBack;

    // Update is called once per frame
    void Update()
    {
        FriendlyDetectCheck();

        if (debugging)
            DebugDrawRaycast();
    }

    void FriendlyDetectCheck()
    {
        friendlyDetectFront = Physics2D.Raycast(attackPoint.position, transform.right, buffRange, enemyLayer);
        friendlyDetectBack = Physics2D.Raycast(attackPoint.position, -transform.right, buffRange, enemyLayer);
    }

    void DebugDrawRaycast()
    {
        Vector3 attackRight = transform.TransformDirection(Vector3.right) * buffRange;
        Debug.DrawRay(attackPoint.position, attackRight, Color.cyan);

        Vector3 attackLeft = transform.TransformDirection(Vector3.left) * buffRange;
        Debug.DrawRay(attackPoint.position, attackLeft, Color.red);
    }
}
