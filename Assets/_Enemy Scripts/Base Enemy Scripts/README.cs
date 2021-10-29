using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class README : MonoBehaviour
{
    [TextArea]
    public string Note = 
        "Add all these scripts (excluding this one) to an enemy. " +
        "[BaseEnemyClass] + [EnemyRaycast] + [EnemyAnimator] + [BaseEnemyController]" +
        "These will provide basic functions, but create a new Controller script and inherit/override from BaseEnemyController.";
}
