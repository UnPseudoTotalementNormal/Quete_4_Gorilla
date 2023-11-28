using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
    private GameObject HUD;

    public float timer;
    [SerializeField] private float maxtimer = 15;
    [SerializeField] private float timerafteraction = 5;

    public GameObject Memberturn;
    private string teamturn;
    private int numturn = 1;

    private GameObject following_object;
    private bool was_following = false;

    private Camera cam;
    private float camZ;

    private void Start()
    {
        HUD = GameObject.Find("HUD");
        cam = Camera.main;
        camZ = cam.transform.position.z;
        EndTurn();
    }

    private void UpdateHUD()
    {
        TextMeshProUGUI hudturn = HUD.transform.Find("TextTurn").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI hudtimer = HUD.transform.Find("TextTimer").GetComponent<TextMeshProUGUI>();
        if (Memberturn != null)
        {
            hudturn.text = teamturn + " " + numturn.ToString() + " turn";
            hudtimer.text = ((int)timer).ToString();
        }
        else
        {
            hudturn.text = " ";
            hudtimer.text = " ";
        }
        
    }

    private void RestartTimer()
    {
        timer = maxtimer;
    }

    private void RunTimer()
    {
        timer -= 1 * Time.fixedDeltaTime;
        if (timer <= 0)
        {
            EndTurn();
        }
    }
    public void ActionTimer()
    {
        if (timer > timerafteraction)
        {
            timer = timerafteraction;
        }
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
        RestartTimer();
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
            case "Monke":
                teamturn = "Bloon";
                break;
            case "Bloon":
                teamturn = "Monke";
                break;
            default:
                teamturn = "Monke";
                break;
        }
        if (GetMember() == null)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private GameObject GetMember()
    {
        return Memberturn = GameObject.Find(teamturn + numturn.ToString()) ;
    }

    private void FixedUpdate()
    {
        UpdateHUD();
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
            RunTimer();
        }
    }

    private void CameraFollow(GameObject following)
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, following.transform.position, 4f * Time.fixedDeltaTime);
        cam.transform.position += new Vector3(0, 0, -cam.transform.position.z + camZ);
    }
}
