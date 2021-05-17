using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTrigger : MonoBehaviour
{
    public StationaryEnemy stationaryEnemy;
    [SerializeField] bool isHealing; //keeps track of when enemy is healing, stops healCO on radius exit
    [SerializeField] bool toggleHeal; //if true heal enemies in radius
    public GameObject healAura;
    Coroutine HealingCO;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            toggleHeal = true;
            HealingCO = StartCoroutine(HealEnemy(collision.gameObject));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isHealing)
        {
            StopCoroutine(HealingCO);
            toggleHeal = false;
            isHealing = false;
        }
    }

    IEnumerator HealEnemy(GameObject enemy)
    {
        while (toggleHeal) {
            isHealing = true;
            enemy.GetComponent<Enemy>().TakeHeal(10);
            Instantiate(healAura, transform.position, Quaternion.identity, transform);
            yield return new WaitForSeconds(1f);
        }
    }
}
