using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindScript : MonoBehaviour
{
    private GameScript gamescript;
    private ParticleSystem windpart;
    private void Start()
    {
        gamescript = GameObject.Find("GameManager").GetComponent<GameScript>();
        windpart = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        var main = windpart.main;
        main.startSpeedMultiplier = gamescript.wind.x * 1.25f;
    }
}
