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
    [SerializeField] int enemyCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(stationaryEnemy != null)
        {
            if (collision.GetComponent<Enemy>() != null || collision.GetComponent<Enemy2>() != null)
            {
                toggleHeal = true;
                if (enemyCount <= 5)
                    enemyCount++;

                HealingCO = StartCoroutine(HealEnemy(collision.gameObject));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (stationaryEnemy != null)
        {
            if (enemyCount > 0)
            {
                if(enemyCount > 0)
                    enemyCount--;
            }
            else
            {
                //enemyCount = 0; //in case of negatives
                toggleHeal = false;
                if(HealingCO != null)
                    StopCoroutine(HealingCO);
            }
        }
    }

    IEnumerator HealEnemy(GameObject enemy)
    {
        while (toggleHeal) {
            if(enemy != null)
            {
                if (enemy.GetComponent<Enemy>() != null)
                {
                    enemy.GetComponent<Enemy>().TakeHeal(10);
                }
                if(enemy.GetComponent<Enemy2>() != null)
                {
                    enemy.GetComponent<Enemy2>().TakeHeal(10);
                }
            }

            Instantiate(healAura, transform.position, Quaternion.identity, transform);
            yield return new WaitForSeconds(1f);
        }
    }
}