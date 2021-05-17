using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	//[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	//[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	public int CurrentGameLevel;

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	public bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	public bool canFlip;

	//public PlayerCombat playerCombat;
	public PlayerMovement movement;
	public Transform TextPopups;
	//[SerializeField] Transform TextPopupsPrefab;
	//RectTransform TextPopupsPrefab; //^prefab uses RectTransform, not Transform, need to reference position/rotation
	public SpriteRenderer playerSR;
	public Transform attackPoint;

	[Header("")]
	[Space]
	public AbilityCycle abilityMovementCycle;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	//public BoolEvent OnCrouchEvent;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		//if (OnCrouchEvent == null)
			//OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				//abilityMovementCycle.ShowAbility(0); //movement ability icon

				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}

		//DisplayGroundedAbility();
	}

	public void Move(float move, bool jump)
	{
        if (movement.canMove)
        {
			//only control the player if grounded or airControl is turned on
			if (m_Grounded || m_AirControl)
			{
				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
				// And then smoothing it out and applying it to the character
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

				// If the input is moving the player right and the player is facing left...
			
				if (move > 0 && !m_FacingRight)
				{
					Flip();
					attackPoint.localScale *= -1;
				}
				else if (move < 0 && m_FacingRight)
				{
					Flip();
					//attackPoint.localScale *= 1; //don't need
				}
			}
			// If the player should jump...
			if (m_Grounded && jump)
			{
				// Add a vertical force to the player.
				m_Grounded = false;
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			}
        }
	}



	public void DisableFlip()
	{
		canFlip = false;
	}

	public void EnableFlip()
	{
		canFlip = true;
	}

	private void Flip()
	{
		if (canFlip) {
			// Switch the way the player is labelled as facing.
			m_FacingRight = !m_FacingRight;

			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}

		/*if (canFlip)
		{
			FacingDirection *= -1;
			transform.Rotate(0.0f, 180.0f, 0.0f);
		}*/
	}

	private void DisplayGroundedAbility()
    {
		/*if(m_Grounded == true) //only using if displaying a separate movement ability when not grounded
        {
			abilityMovementCycle.ShowAbilities(0, 1); //display first ability
		}
        else
        {
			abilityMovementCycle.ShowAbilities(0, 1, false); //display second ability, false to flip the default false
		}*/
	}

	//RevivePlayer in PlayerCombat
	public void RespawnPlayerResetLevel()
    {
		CurrentGameLevel = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(CurrentGameLevel);
	}
}