using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTrigger : MonoBehaviour
{
    public StationaryEnemy stationaryEnemy;
    [SerializeField] bool isHealing;
    public GameObject healAura;
    Coroutine HealingCO;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered");
        if(collision.GetComponent<Enemy>() != null)
        {
            isHealing = true;
            HealingCO = StartCoroutine(HealEnemy(collision.gameObject));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit");
        if (isHealing)
        {
            StopCoroutine(HealingCO);
            isHealing = false;
        }
    }

    IEnumerator HealEnemy(GameObject enemy)
    {
        enemy.GetComponent<Enemy>().TakeHeal(5);
        Instantiate(healAura, transform.position, Quaternion.identity, transform);
        yield return new WaitForSeconds(1f);
    }

    /*
    void ToggleHeal(GameObject collider, bool isHealing)
    {
        if (isHealing)
        {
            if (collider.GetComponent<Enemy>() != null)
            {
                //collider.GetComponent<Enemy>().TakeHeal(5);
                HealingCO = StartCoroutine(Healing(collider, 5));
                //isHealing = true;
            }

            if(collider.GetComponent<Enemy2>() != null)
            {
                collider.GetComponent<Enemy2>().TakeHeal(5);
            }
        }
        else
        {
            StopCoroutine(HealingCO);
        }
    }

    IEnumerator Healing(GameObject friend, float healAmount)
    {

        friend.GetComponent<Enemy>().TakeHeal(5);
        Instantiate(healAura, transform.position, Quaternion.identity, transform);
        yield return new WaitForSeconds(1f);
        
    }*/
}
