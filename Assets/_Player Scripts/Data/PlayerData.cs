using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]

public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float movementVelocity = 10f;

    [Header("Jump State")]
    public float jumpVelocity = 15f;
    public int amountOfJumps = 1; //if we want double jump

    [Header("In Air State")]
    public float coyoteTime = 0.02f;
    public float variableJumpHeightMultiplier = 0.5f;

    [Header("Dash State")]
    public float dashCooldown = 1.0f;
    public float dashTime = 0.2f;
    public float dashVelocity = 10f;
    public float distBetweenAfterImages = 0.5f;

    [Header("Check Variables")]
    public float groundCheckRadius = 0.3f;
    public LayerMask whatIsGround;
}
