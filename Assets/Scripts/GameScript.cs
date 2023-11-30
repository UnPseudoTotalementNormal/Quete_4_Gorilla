using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
    [SerializeField] private GameObject MONKE;
    [SerializeField] private GameObject BLOON;

    private GameObject HUD;

    public float timer;
    [SerializeField] private float maxtimer = 15;
    [SerializeField] private float timerafteraction = 5;

    public GameObject Memberturn;
    public string teamturn;
    private int numturn = 1;

    private int wave = 1;

    private GameObject following_object;
    private GameObject following_object2;
    private bool was_following = false;

    private Camera cam;
    private float camZ;

    [SerializeField] public Vector2 wind;

    int map_min;
    int map_max;
    int map_y;
    private void Start()
    {
        map_min = GameObject.Find("Map2").GetComponent<Map2script>().startX;
        map_max = GameObject.Find("Map2").GetComponent<Map2script>().width - 1;
        map_y = GameObject.Find("Map2").GetComponent<Map2script>().minHeight;
        HUD = GameObject.Find("HUD");
        cam = Camera.main;
        camZ = cam.transform.position.z;
        RandomizeWind();
        EndTurn();
    }

    private void SpawnEntity(string entity, Vector2 pos, int hp)
    {
        if (pos == Vector2.zero)
        {
            pos = new Vector2(UnityEngine.Random.Range(map_min, map_max), map_y + 3);
        }
        int i = 1;
        string newname;
        switch (entity)
        {
            case "Monke":
                while (GameObject.Find("Monke"+i.ToString()) != null)
                {
                    i++;
                }
                newname = "Monke" + i.ToString();
                GameObject newmonke = Instantiate(MONKE, pos, Quaternion.identity);
                newmonke.name = newname;
                newmonke.GetComponent<HealthComponent>().health = hp;
                break;
            case "Bloon":
                while (GameObject.Find("Bloon" + i.ToString()) != null)
                {
                    i++;
                }
                newname = "Bloon" + i.ToString();
                GameObject newbloon = Instantiate(BLOON, pos, Quaternion.identity);
                newbloon.name = newname;
                newbloon.GetComponent<HealthComponent>().health = hp;
                break;
        }
    }

    private void RandomizeWind()
    {
        float r = UnityEngine.Random.Range(-7, 7);
        wind = new Vector2(r, 0);
        if (wind.x > 0)
        {
            HUD.transform.Find("WindArrow").transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            HUD.transform.Find("WindArrow").transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        HUD.transform.Find("WindText").GetComponent<TextMeshProUGUI>().text = Mathf.Abs(wind.x).ToString() + " m/s";
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

    public void FollowThis(GameObject follow_this = null, GameObject follow_this2 = null)
    {
        if (follow_this != null)
        {
            following_object = follow_this;
            following_object2 = follow_this2;
            return;
        }
    }
    public void EndTurn(GameObject follow_this = null, GameObject follow_this2 = null)
    {
        Memberturn = null;
        if (follow_this != null)
        {
            was_following = true;
            following_object = follow_this;
            following_object2 = follow_this2;
            return;
        }
        StartCoroutine(CodeAfterDelay(NextTurn, 3f));
        
    }

    private void NextTurn()
    {
        following_object = null;
        following_object2 = null;
        RandomizeWind();
        RestartTimer();
        numturn++;
        if (GetMember(true) == null)
        {
            ChangeTeam();
        }
    }

    private void NextWave()
    {
        wave += 1;
        switch (wave)
        {
            case 2:
                SpawnEntity("Bloon", Vector2.zero, 1);
                SpawnEntity("Bloon", Vector2.zero, 1);
                break;
            case 3:
                SpawnEntity("Bloon", Vector2.zero, 2);
                break;
            case 4:
                SpawnEntity("Bloon", Vector2.zero, 1);
                SpawnEntity("Bloon", Vector2.zero, 1);
                break;
            case 5:
                SpawnEntity("Bloon", Vector2.zero, 3);
                break;
            case 6:
                SpawnEntity("Bloon", Vector2.zero, 1);
                SpawnEntity("Bloon", Vector2.zero, 2);
                break;
            default:
                SpawnEntity("Bloon", Vector2.zero, 1);
                break;
        }
    }
    IEnumerator CodeAfterDelay(Action nextfunction, float delay)
    {
        yield return new WaitForSeconds(delay);
        nextfunction();
    }

    private void ChangeTeam()
    {
        following_object = null;
        following_object2 = null;
        Memberturn = null;
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
        if (GetMember(true) == null)
        {
            switch (teamturn)
            {
                case "Monke":
                    SceneManager.LoadScene("MainMenu");
                    break;
                case "Bloon":
                    NextWave();
                    FollowThis(GetMember(false));
                    StartCoroutine(CodeAfterDelay(ChangeTeam, 2));
                    break;
            }
            
        }
    }

    private GameObject GetMember(bool set = true)
    {
        if (set) 
        {
            return Memberturn = GameObject.Find(teamturn + numturn.ToString());
        }
        else
        {
            return GameObject.Find(teamturn + numturn.ToString());
        }
    }

    private void FixedUpdate()
    {
        UpdateHUD();
        if (following_object !=  null)
        {
            if (following_object2 != null)
            {
                CameraFollow(following_object, following_object2);
            }
            else
            {
                CameraFollow(following_object);
            }
        }
        else if (was_following)
        {
            was_following = false;
            StartCoroutine(CodeAfterDelay(NextTurn, 3f));
        }

        if (Memberturn != null)
        {
            if (following_object2 == null)
            {
                CameraFollow(Memberturn);
            }
            RunTimer();
        }
    }

    private void CameraFollow(GameObject following, GameObject following2 = null)
    {
        
        if (following2 == null)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, following.transform.position, 4f * Time.fixedDeltaTime);
            cam.transform.position += new Vector3(0, 0, -cam.transform.position.z + camZ);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 9, 5f * Time.fixedDeltaTime);
        }
        else
        {
            float distance = (following.transform.position - following2.transform.position).magnitude;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, distance/2.25f, 5f * Time.fixedDeltaTime);
            cam.transform.position = (following.transform.position + following2.transform.position) / 2;
            cam.transform.position += new Vector3(0, 0, -cam.transform.position.z + camZ);
        }
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 9, 14);
    }
}
