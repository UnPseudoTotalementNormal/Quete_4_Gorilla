using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
    private GameScript gamemanager;
    void Start()
    {
        gamemanager = GameObject.Find("GameManager").GetComponent<GameScript>();
    }

    void Update()
    {
        
    }

    public void QuiShop()
    {
        gamemanager.inshop = false;
        Time.timeScale = 1;
    }
}
