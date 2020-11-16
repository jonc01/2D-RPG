using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingDamageController : MonoBehaviour
{
    private static FloatingDamage popupText;
    private static GameObject canvas;

    public static void Initialize()
    {
        canvas = GameObject.Find("Canvas");
        if(!popupText)
            popupText = Resources.Load<FloatingDamage>("_Prefabs/TextPopups");
    }
    public static void CreateFloatingText(string text, Transform location)
    {
        FloatingDamage instance = Instantiate(popupText);
        instance.transform.SetParent(canvas.transform, false);
        instance.SetText(text);
    }
}
