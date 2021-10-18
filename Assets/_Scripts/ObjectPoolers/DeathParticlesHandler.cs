using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticlesHandler : MonoBehaviour
{
    [SerializeField]
    private ObjectPoolerList DeathParticlesPool;

    public void ShowHitEffect(Vector3 position)
    {
        GameObject showParticles = DeathParticlesPool.GetObject();
        showParticles.transform.position = position;
        showParticles.transform.rotation = Quaternion.identity;
        showParticles.SetActive(true);
    }
}
