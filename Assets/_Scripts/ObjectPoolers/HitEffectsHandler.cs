using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectsHandler : MonoBehaviour
{
    [SerializeField]
    private ObjectPoolerList HitEffectsPool;

    public void ShowHitEffect(Vector3 position)
    {
        GameObject showHit = HitEffectsPool.GetObject();
        showHit.transform.position = position;
        showHit.transform.rotation = Quaternion.identity;
        showHit.SetActive(true);
    }
}