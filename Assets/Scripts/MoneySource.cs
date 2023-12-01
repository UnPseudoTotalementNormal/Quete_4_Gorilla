using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySource : MonoBehaviour
{
    private GameScript gamemanager;

    [SerializeField] int amount = 1;

    private void Start()
    {
        gamemanager = GameObject.Find("GameManager").GetComponent<GameScript>();
    }

    public void GiveMoney()
    {
        gamemanager.monke_money += amount;
    }
}
