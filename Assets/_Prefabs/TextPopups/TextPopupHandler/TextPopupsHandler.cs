using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopupsHandler : MonoBehaviour
{
    public GameObject TextPopupsPrefab;
    public float DestroyTime = 1.6f; //.8f
    public float yOffset = 2f;

    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }

    public void ShowDamage(float damage, Vector3 position)
    {
        //Vector3 tempTransform = transform.position; //randomize damage number position
        /*Vector3 tempPos = transform.position;
        tempPos.x += Random.Range(-.1f, .1f);
        tempPos.y += Random.Range(-.9f, .1f);*/
        
        /*Vector3 tempPos = position;
        tempPos.z = 0;*/

        var showDmg = Instantiate(TextPopupsPrefab, position, Quaternion.identity);

        if (damage > 0)
        {
            showDmg.GetComponent<TextMeshPro>().text = damage.ToString();
        }
        else if(damage < 0)
        {
            showDmg.GetComponent<TextMeshPro>().text = damage.ToString();
            showDmg.GetComponent<TextMeshPro>().color = new Color32(35, 220, 0, 255);
        }
        else //grey for 0 damage?
        {
            showDmg.GetComponent<TextMeshPro>().text = damage.ToString();
        }
        //tempShowDmg = showDmg;
    }

    public void ShowHeal(float heal, Vector3 position)
    {
        var showHeal = Instantiate(TextPopupsPrefab, position, Quaternion.identity);
        showHeal.GetComponent<TextMeshPro>().text = heal.ToString();
        showHeal.GetComponent<TextMeshPro>().color = new Color32(35, 220, 0, 255);
    }

    public void ShowDodge(Vector3 position)
    {
        var showDmg = Instantiate(TextPopupsPrefab, position, Quaternion.identity);
        showDmg.GetComponent<TextMeshPro>().text = "Dodged";
    }
}
