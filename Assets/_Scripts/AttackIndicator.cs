using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    [SerializeField]
    private float DestroyTime = .5f;
    //public Transform RectTransform;

    [SerializeField]
    private ObjectPoolerList pool;

    void Start()
    {
        pool = transform.parent.GetComponent<ObjectPoolerList>();
    }

    private void OnEnable()
    {
        StartCoroutine(PoolTextPopups());
    }

    IEnumerator PoolTextPopups()
    {
        yield return new WaitForSeconds(DestroyTime);
        pool.ReturnObject(gameObject);
    }
}
