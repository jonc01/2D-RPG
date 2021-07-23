using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpen : MonoBehaviour
{
    //public Animator animator;
    public GameObject closedChest;
    public GameObject openedChest;
    public GameObject textPrompt;
    public Interactable interactableScript;
    public bool isOpen = false;


    // Start is called before the first frame update
    void Start()
    {
        //currentHealth = maxHealth;
        
        closedChest.GetComponent<SpriteRenderer>().enabled = true;
        openedChest.GetComponent<SpriteRenderer>().enabled = false;
        textPrompt.GetComponent<Canvas>().enabled = false;
        isOpen = false;
    }

    void Update()
    {
        if (interactableScript.isInRange == true && !isOpen)
        {
            textPrompt.GetComponent<Canvas>().enabled = true;
            //Debug.Log("ChestOpen script: in range");
        }
        else
        {
            textPrompt.GetComponent<Canvas>().enabled = false;
            //Debug.Log("ChestOpen script: not in range");
        }
    }

    public void Open()
    {
        closedChest.GetComponent<SpriteRenderer>().enabled = false;
        openedChest.GetComponent<SpriteRenderer>().enabled = true;
        //give items
        textPrompt.GetComponent<Canvas>().enabled = false;
        isOpen = true;
        //Destroy(textPrompt);

    }

}
