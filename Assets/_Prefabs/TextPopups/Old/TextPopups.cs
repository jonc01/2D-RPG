using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPopups : MonoBehaviour
{
    public float DestroyTime = 1.6f;
    //public Transform RectTransform;

    [SerializeField]
    private ObjectPoolerList pool;

    void Start()
    {
        pool = transform.parent.GetComponent<ObjectPoolerList>();
        pool.ReturnObject(gameObject);

        //Destroy(gameObject, DestroyTime);
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
