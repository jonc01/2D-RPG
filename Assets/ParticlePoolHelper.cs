using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePoolHelper : MonoBehaviour
{
    [SerializeField]
    private ObjectPoolerList pool; //Don't need to manual set

    void Start()
    {
        pool = transform.parent.GetComponent<ObjectPoolerList>();
    }

    private void OnParticleSystemStopped()
    {
        pool.ReturnObject(gameObject);
    }
}
