using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPopups : MonoBehaviour
{
    public float DestroyTime = 1.0f;
    //public Transform RectTransform;

    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }
}
