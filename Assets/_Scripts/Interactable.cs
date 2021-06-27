using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool isInRange;
    public UnityEvent interactAction;

    public EnemyBossBandit boss;
    //public bool bossReward = true; //if interactable is only available after boss was defeated


    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<EnemyBossBandit>();
    }

    void Update()
    {
        if (boss && isInRange && !boss.isAlive)
        {
            if (Input.GetButtonDown("Interact"))
            {
                interactAction.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
            //Debug.Log("Interactable script: in range");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = false;
            //Debug.Log("Interactable script: not in range");
        }
    }

}
