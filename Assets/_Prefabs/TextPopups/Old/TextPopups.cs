using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPopups : MonoBehaviour
{
    public float DestroyTime = .8f;
    public Vector3 Offset = new Vector3(0, 2, 0);
    //public Transform RectTransform;

    void Start()
    {
        Destroy(gameObject, DestroyTime);

        //RectTransform myRT = GetComponent<RectTransform>();
        //myRT.localPosition += new Vector3(0, 2, 0); //can move left/right, can't change Y
        
        /*RectTransform rt = gameObject.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector3 (0, 2, 0);*/

    }

    public void FlipText(float rotate)
    {
        RectTransform myRotate = GetComponent<RectTransform>();
        RectTransform tempRotate = myRotate;
        tempRotate.localRotation = Quaternion.Euler(0, rotate, 0); //rotate 180 or 0
        myRotate.localRotation = tempRotate.localRotation;
    }
}
