using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffects : MonoBehaviour
{
    [SerializeField]
    private float DestroyTime = 0.5f;

    [SerializeField]
    private ObjectPoolerList pool;

    void Start()
    {
        pool = transform.parent.GetComponent<ObjectPoolerList>();
    }

    private void OnEnable()
    {
        StartCoroutine(PoolObject());
    }

    IEnumerator PoolObject()
    {
        yield return new WaitForSeconds(DestroyTime);
        pool.ReturnObject(gameObject);
    }
}
