using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    private GameObject Memberturn;
    private string teamturn;
    private int numturn = 1;

    private Camera cam;
    private float camZ;

    private void Start()
    {
        cam = Camera.main;
        camZ = cam.transform.position.z;
        ChangeTeam();
        GetMember();
    }

    public void EndTurn()
    {
        Memberturn = null;
        StartCoroutine(CodeAfterDelay(NextTurn, 3f));
        
    }

    private void NextTurn()
    {
        numturn++;
        if (GetMember() == null)
        {
            ChangeTeam();
        }
    }
    IEnumerator CodeAfterDelay(Action nextfunction, float delay)
    {
        yield return new WaitForSeconds(delay);
        nextfunction();
    }

    private void ChangeTeam()
    {
        numturn = 1;
        switch (teamturn)
        {
            case "Player":
                teamturn = "Ennemi";
                break;
            case "Ennemi":
                teamturn = "Player";
                break;
            default:
                teamturn = "Player";
                break;
        }
        GetMember();
    }

    private GameObject GetMember()
    {
        return Memberturn = GameObject.Find(teamturn + numturn.ToString()) ;
    }

    private void FixedUpdate()
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, Memberturn.transform.position, 3f * Time.fixedDeltaTime);
        cam.transform.position += new Vector3(0, 0, -cam.transform.position.z + camZ);
    }
}
