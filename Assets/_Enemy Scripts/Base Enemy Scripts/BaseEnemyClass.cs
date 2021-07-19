using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyClass : MonoBehaviour
{
    // TODO:
    //[SerializeField] EnemyStateController state


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TakeDamage()
    {
        //state.ChangeState(EN_HURT); //TODO: State Controller should recognize this as the const stored
    }

    public void GetStunned()
    {

    }

    public void Die()
    {

    }
}
