using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField]
    private bool combatEnabled;
    [SerializeField]
    private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField]
    private Transform attack1HitBoxPos;
    [SerializeField]
    private LayerMask whatIsDamageable;


    private bool gotInput;
    private bool isAttacking, isFirstAttack;

    private float lastInputTime = Mathf.NegativeInfinity;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled); //setting animator var while enabling combat

        //attack1Damage = weapon.damage;
        //attack2Damage = weapon.damage;
        //attack3Damage = weapon.damage *= 1.5f;
    }

    private void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }

    private void CheckCombatInput()
    {
        //if (Input.GetMouseButtonDown(0)) //left mouse, change to allow for rebinding
        //if(Input.GetButtonDown("Attack"))
        var keyboard = Keyboard.current;

        //if (Input.GetButtonDown("Fire1") && m_timeSinceAttack > 0.25f && canAttack)
        if (keyboard.jKey.isPressed)
        {
            if (combatEnabled)
            {
                Debug.Log("attacking.......");
                gotInput = true;
                lastInputTime = Time.time;
                //anim.SetBool("idle", false); 
                //TODO: Player FSM need to re-set this transition to idle after attack ends
                    //! currently not using "idle" with old player controller
            }
        }
    }

    private void CheckAttacks()
    {
        if (gotInput)
        {
            //attack 1
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                //anim.SetBool("attack1", true); //TODO: possible change to bool instead of trigger
                anim.SetTrigger("Attack1");
                anim.SetBool("firstAttack", isFirstAttack);
                anim.SetBool("isAttacking", isAttacking);
            }
        }

        if(Time.time >= lastInputTime + inputTimer)
        {
            //wait for new input
            gotInput = false;
        }

    }

    private void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attack1Damage);
            //Instantiate player's hit effect, can change depending on weapon
        }
    }

    private void FinishAttack1()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        //anim.SetBool("attack1", false);
        anim.SetTrigger("Attack1");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }
}
