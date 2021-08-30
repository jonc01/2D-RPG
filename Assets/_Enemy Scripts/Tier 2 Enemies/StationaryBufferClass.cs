using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryBufferClass : BaseEnemyClass
{
    [Space]
    [Header("=== Stationary Buffer Only ===")]
    public LayerMask friendlyLayer;
    public GameObject ownHitParticle;
    public GameObject healParticle;
    public EffectAnimatorManager effectAnimator;

    [SerializeField]
    protected float
        buffDamage = 10f,
        buffCastSpeed = .5f,
        buffRadius = .3f;
    bool isBuffing;
    Coroutine BuffingCO = null;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        isBuffing = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //overriding animCheck in base, do nothing
    }

    public void StartBuffing()
    {
        if (!isBuffing)
            BuffingCO = StartCoroutine(Buffing());
    }

    IEnumerator Buffing()
    {
        isBuffing = true;
        effectAnimator.HealAura();
        yield return new WaitForSeconds(.4f);
        Buff();
        yield return new WaitForSeconds(buffCastSpeed);
        isBuffing = false;
    }

    void Buff()
    {
        Collider2D[] hitFriendly = Physics2D.OverlapCircleAll(enAttackPoint.position, buffRadius, friendlyLayer);

        foreach (Collider2D friendly in hitFriendly) //loop through enemies hit
        {
            if (friendly.GetComponent<BaseEnemyClass>() != null)
            {
                //if(healParticle != null)
                //    Instantiate(healParticle, transform.position, Quaternion.identity, transform);
                //TODO: need to re-add healParticle, or find animation to replace

                friendly.GetComponent<BaseEnemyClass>().GetHealed(buffDamage); //attackDamage + additional damage from parameter
            }
        
            //if(friendly.GetComponent<PlayerCombat>() != null)
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (enAttackPoint == null)
            return;

        Gizmos.DrawWireSphere(enAttackPoint.position, buffRadius);
    }

    public override void GetKnockback(bool playerFacingRight, float thrustMult = 1f)//, float kbDuration = 5f) //defaults
    {
        //playerFacingRight - passed from PlayerCombat when damage is applied
        //kbThrust - velocity of lunge movement
        //kbDuration - how long to maintain thrust velocity (distance)
        Vector3 particleLocation = transform.position;
        particleLocation.y -= hitEffectOffset;

        if (ownHitParticle != null)
            Instantiate(ownHitParticle, particleLocation, Quaternion.identity, transform);
    }

    public override void GetHealed(float healAmount)
    {
        //do nothing
    }

    public override void Die()
    {
        isAlive = false;

        StopAllCoroutines();

        if (DeathParticlesHandler != null)
        {
            Vector3 tempLocation = transform.position;
            tempLocation.y += hitEffectOffset;

            DeathParticlesHandler.ShowHitEffect(tempLocation);
        }
        StartCoroutine(DeleteEnemyObject());
    }

    IEnumerator DeleteEnemyObject()
    {
        sr.enabled = false;
        GetComponentInChildren<Canvas>().enabled = false; //removes health bar
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}
