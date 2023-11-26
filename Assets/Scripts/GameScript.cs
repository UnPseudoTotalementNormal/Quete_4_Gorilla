using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public GameObject Memberturn;
    private string teamturn;
    private int numturn = 1;

    private GameObject following_object;
    private bool was_following = false;

    private Camera cam;
    private float camZ;

    private void Start()
    {
        cam = Camera.main;
        camZ = cam.transform.position.z;
        EndTurn();
    }

    public void EndTurn(GameObject follow_this = null)
    {
        Memberturn = null;
        if (follow_this != null)
        {
            was_following = true;
            following_object = follow_this;
            return;
        }
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
        if (following_object !=  null)
        {
            CameraFollow(following_object);
        }
        else if (was_following)
        {
            was_following = false;
            StartCoroutine(CodeAfterDelay(NextTurn, 3f));
        }

        if (Memberturn != null)
        {
            CameraFollow(Memberturn);
        }
    }

    private void CameraFollow(GameObject following)
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, following.transform.position, 4f * Time.fixedDeltaTime);
        cam.transform.position += new Vector3(0, 0, -cam.transform.position.z + camZ);
    }
}
