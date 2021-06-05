using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopupsHandler : MonoBehaviour
{
    //public float DestroyTime = 1.6f; //.8f
    //public float yOffset = 2f;
    //[SerializeField] GameObject TextPopupsCanvas;

    [SerializeField]
    private ObjectPoolerList TextPopupsPool;

    public void ShowDamage(float damage, Vector3 position, bool crit = false)
    {
        //Vector3 tempTransform = transform.position; //randomize damage number position
        /*Vector3 tempPos = transform.position;
        tempPos.x += Random.Range(-.1f, .1f);
        tempPos.y += Random.Range(-.9f, .1f);*/

        float fontSize = 1.2f;

        GameObject showDmg = TextPopupsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);
        showDmg.GetComponent<TextMeshProUGUI>().fontSize = fontSize; //resetting to normal font size

        if (damage > 0)
        {
            if (crit)
            {
                showDmg.GetComponent<TextMeshProUGUI>().color = new Color32(255, 70, 0, 255); //orange text //255, 120
                showDmg.GetComponent<TextMeshProUGUI>().fontSize = fontSize*1.5f;
                //showDmg.GetComponent<Animator>().SetBool
                //showDmg.GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255); //red text
            }
            else
            {
                showDmg.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255); //white text
            }
            showDmg.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        }
        else if(damage < 0) //healing
        {
            showDmg.GetComponent<TextMeshProUGUI>().text = Mathf.Abs(damage).ToString();
            showDmg.GetComponent<TextMeshProUGUI>().color = new Color32(35, 220, 0, 255); //green text
        }
        else //0 damage, blocked
        {
            showDmg.GetComponent<TextMeshProUGUI>().text = "Blocked!";
            showDmg.GetComponent<TextMeshProUGUI>().color = new Color32(188, 188, 188, 255); //grey text
        }
    }

    public void ShowHeal(float heal, Vector3 position)
    {
        GameObject showDmg = TextPopupsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);

        showDmg.GetComponent<TextMeshProUGUI>().text = heal.ToString();
        showDmg.GetComponent<TextMeshProUGUI>().color = new Color32(35, 220, 0, 255);
        showDmg.GetComponent<TextMeshProUGUI>().fontSize = 1.2f;
    }

    public void ShowDodge(Vector3 position)
    {
        GameObject showDmg = TextPopupsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);
        showDmg.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255); //white text
        showDmg.GetComponent<TextMeshProUGUI>().text = "Dodged";
        showDmg.GetComponent<TextMeshProUGUI>().fontSize = 1.2f;
    }

    public void ShowStun(Vector3 position, float defaultFont = 1.2f)
    {
        position.y += .25f;

        GameObject showDmg = TextPopupsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);
        showDmg.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255); //white text
        showDmg.GetComponent<TextMeshProUGUI>().text = "*Stun*";
        showDmg.GetComponent<TextMeshProUGUI>().fontSize = defaultFont;
    }
    
    public void ShowBreak(Vector3 position)
    {
        position.y -= .65f;

        GameObject showDmg = TextPopupsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);
        showDmg.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255); //white text
        showDmg.GetComponent<TextMeshProUGUI>().text = "Break";
        showDmg.GetComponent<TextMeshProUGUI>().fontSize = 1f;
    }

    public void ShowText(Vector3 position, string text, float defaultFont = 1.2f)
    {
        position.y += .25f;

        GameObject showDmg = TextPopupsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);
        showDmg.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        showDmg.GetComponent<TextMeshProUGUI>().text = text;
        showDmg.GetComponent<TextMeshProUGUI>().fontSize = defaultFont;
    }
}
