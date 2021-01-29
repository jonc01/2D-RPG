using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectsHandler : MonoBehaviour
{
    [SerializeField]
    private ObjectPoolerList HitEffectsPool;


    public void ShowHitEffect(Vector3 position)
    {

        GameObject showDmg = HitEffectsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);
    }

    public void ShowDodge(Vector3 position)
    {
        GameObject showDmg = HitEffectsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);
    }

    public void ShowStun(Vector3 position)
    {
        position.y += .25f;

        GameObject showDmg = HitEffectsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);
    }
}
