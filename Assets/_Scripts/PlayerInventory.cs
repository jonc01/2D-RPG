using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{


    //Stats
    //public float totalXP; //currently setup in PlayerCombat
    public int totalGold;

    public float healthBuff; //TODO: not setup with playerCombat
    public float damageBuff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void GiveXP(float xp) //might not need
    {
        totalXP += xp;
    }*/
    public void GiveGold(int gold)
    {
        totalGold += gold;
    }
}
