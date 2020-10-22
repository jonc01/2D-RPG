using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    //Script by Code Monkey via youtube
    //makes assets easier to reference without dragging into object fields

    private static GameAssets _i;

    public static GameAssets i {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }
    }

    //Asset References here:
    public Transform pfDamagePopup;
    
    
}
