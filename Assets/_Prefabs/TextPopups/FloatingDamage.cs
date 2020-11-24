using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;

public class FloatingDamage : MonoBehaviour
{
    public Animator anim;
    private Text damageText;

    // Start is called before the first frame update
    void Start()
    {
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
        damageText = anim.GetComponent<Text>();
    }

    public void SetText(string text)
    {
        damageText.text = text;
    }
}
