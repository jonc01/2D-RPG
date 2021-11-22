using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerXPManager : MonoBehaviour
{
    public static PlayerXPManager Instance { get; private set; }
    [SerializeField] public PlayerCombat playerCombat;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (playerCombat == null)
            playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();

    }
}
