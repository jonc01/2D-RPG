﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopupsHandler : MonoBehaviour
{
    public GameObject TextPopupsPrefab;
    //public float DestroyTime = 1.6f; //.8f
    public float yOffset = 2f;
    //[SerializeField] GameObject TextPopupsCanvas;

    [SerializeField]
    private ObjectPoolerList TextPopupsPool;


    void Start()
    {
        //TextPopupsPool = transform.parent.GetComponent<ObjectPoolerList>();
        //TextPopupsPool.ReturnObject(gameObject);

        //Destroy(gameObject, DestroyTime);
    }

    public void ShowDamage(float damage, Vector3 position, bool crit = false)
    {
        //Vector3 tempTransform = transform.position; //randomize damage number position
        /*Vector3 tempPos = transform.position;
        tempPos.x += Random.Range(-.1f, .1f);
        tempPos.y += Random.Range(-.9f, .1f);*/

        GameObject showDmg = TextPopupsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);

        if (damage > 0)
        {
            if (crit)
            {
                showDmg.GetComponent<TextMeshPro>().color = new Color32(255, 0, 0, 255); //red text
            }
            else
            {
                showDmg.GetComponent<TextMeshPro>().color = new Color32(255, 255, 255, 255); //white text
            }
            showDmg.GetComponent<TextMeshPro>().text = damage.ToString();
        }
        else if(damage < 0)
        {
            showDmg.GetComponent<TextMeshPro>().text = Mathf.Abs(damage).ToString();
            showDmg.GetComponent<TextMeshPro>().color = new Color32(35, 220, 0, 255); //green text
        }
        else //grey for 0 damage?
        {
            showDmg.GetComponent<TextMeshPro>().text = damage.ToString();
            showDmg.GetComponent<TextMeshPro>().color = new Color32(255, 255, 255, 255); //white text
        }
    }

    public void ShowHeal(float heal, Vector3 position)
    {
        GameObject showDmg = TextPopupsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);

        showDmg.GetComponent<TextMeshPro>().text = heal.ToString();
        showDmg.GetComponent<TextMeshPro>().color = new Color32(35, 220, 0, 255);
    }
    
    public void ShowDodge(Vector3 position)
    {
        GameObject showDmg = TextPopupsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);
        showDmg.GetComponent<TextMeshPro>().color = new Color32(255, 255, 255, 255); //white text
        showDmg.GetComponent<TextMeshPro>().text = "Dodged";
    }

    public void ShowStun(Vector3 position)
    {
        position.y += .25f;

        GameObject showDmg = TextPopupsPool.GetObject();
        showDmg.transform.position = position;
        showDmg.transform.rotation = Quaternion.identity;
        showDmg.SetActive(true);
        showDmg.GetComponent<TextMeshPro>().color = new Color32(255, 255, 255, 255); //white text
        showDmg.GetComponent<TextMeshPro>().text = "*Stun*";
    }
}
