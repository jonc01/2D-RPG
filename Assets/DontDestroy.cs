using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    //https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("MultiScene");

        if(objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
