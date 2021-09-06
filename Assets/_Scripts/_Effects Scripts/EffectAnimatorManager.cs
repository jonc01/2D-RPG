using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimatorManager : MonoBehaviour
{
    [SerializeField] Animator effects;

    // Start is called before the first frame update
    void Start()
    {
        effects = transform.GetComponent<Animator>();
    }

    public void HealAura()
    {
        effects.SetTrigger("HealAura");
    }

    public void Pulse()
    {
        effects.SetTrigger("Pulse");
    }

    public void Vortex(bool loopAnim = false, bool setAnim = true)
    {
        if (!loopAnim)
        {
            effects.SetTrigger("Vortex");
        }
        else
        {
            effects.SetBool("LoopVortex", setAnim);
        }
    }

}
