using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    //https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html
    //Objects with the tag AND script won't be destroyed, unless multiple objects with the tag have the script.
    //In this case, the duplicate is deleted.

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
