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

    private Transform monkes_folder;
    private Transform bloons_folder;

    private GameObject HUD;

    public float timer;
    [SerializeField] private float maxtimer = 15;
    [SerializeField] private float timerafteraction = 5;

    public GameObject Memberturn;
    public string teamturn;
    private int numturn = 1;

    [SerializeField] private int wave = 1;

    private GameObject following_object;
    private GameObject following_object2;
    private bool was_following = false;

    private Camera cam;
    private float camZ;

    [SerializeField] public Vector2 wind;

    int map_min;
    int map_max;
    int map_y;

    private Transform shopmenu;

    public bool inshop = false;
    public int monke_money = 0;

    public int camera_normal_zoom = 9;
    private void Start()
    {
        monkes_folder = transform.Find("Monkes");
        bloons_folder = transform.Find("Bloons");
        map_min = GameObject.Find("Map2").GetComponent<Map2script>().startX;
        map_max = GameObject.Find("Map2").GetComponent<Map2script>().width - 1;
        map_y = GameObject.Find("Map2").GetComponent<Map2script>().minHeight;
        HUD = GameObject.Find("HUD");
        shopmenu = HUD.transform.Find("Shop");
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
                GameObject newmonke = Instantiate(MONKE, pos, Quaternion.identity, monkes_folder);
                newmonke.name = newname;
                newmonke.GetComponent<HealthComponent>().health = hp;
                break;
            case "Bloon":
                while (GameObject.Find("Bloon" + i.ToString()) != null)
                {
                    i++;
                }
                newname = "Bloon" + i.ToString();
                GameObject newbloon = Instantiate(BLOON, pos, Quaternion.identity, bloons_folder);
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
        TextMeshProUGUI hudwave = HUD.transform.Find("TextWave").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI hudmoney = HUD.transform.Find("TextMoney").GetComponent<TextMeshProUGUI>();
        hudwave.text = "Wave: " + wave.ToString();
        if (Memberturn != null)
        {
            hudturn.text = teamturn + " " + numturn.ToString() + " turn";
            hudtimer.text = ((int)timer).ToString();
            hudmoney.text = "$" + monke_money.ToString();
        }
        else
        {
            hudturn.text = " ";
            hudtimer.text = " ";
        }
        
        if (inshop)
        {
            shopmenu.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Lerp(shopmenu.GetComponent<RectTransform>().localPosition.x, 0, 1f * Time.fixedDeltaTime), 0, 0);
        }
        else
        {
            shopmenu.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Lerp(shopmenu.GetComponent<RectTransform>().localPosition.x, 215 * cam.orthographicSize, 1f * Time.fixedDeltaTime), 0, 0);
        }
    }

    private void OpenShop()
    {
        Time.timeScale = 0f;
        inshop = true;
        shopmenu.GetComponent<ShopScript>().OpenShop();
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

    
    IEnumerator CodeAfterDelay(Action nextfunction, float delay)
    {
        yield return new WaitForSeconds(delay);
        nextfunction();
    }

    private void ChangeTeam()
    {
        setmembersready();
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
                    OpenShop();
                    NextWave();
                    RefillMagicShields();
                    FollowThis(GetMember(false));
                    StartCoroutine(CodeAfterDelay(ChangeTeam, 2));
                    break;
            }
            
        }
    }

    private void setmembersready()
    {
        for (int i = 0; i < monkes_folder.childCount; i++)
        {
            monkes_folder.GetChild(i).GetComponent<player>().played_this_turn = false;
        }

        for (int i = 0; i < bloons_folder.childCount; i++)
        {
            bloons_folder.GetChild(i).GetComponent<EnnemiScript>().played_this_turn = false;
        }
    }

    private GameObject GetMember(bool set = true)
    {
        GameObject searchmember = null;
        if (teamturn == "Monke")
        {
            for (int i = 0; i < monkes_folder.childCount; i++)
            {
                if (monkes_folder.GetChild(i).GetComponent<player>().played_this_turn == false)
                {
                    searchmember = monkes_folder.GetChild(i).gameObject;
                }
            }
        }
        else if (teamturn == "Bloon")
        {
            for (int i = 0; i < bloons_folder.childCount; i++)
            {
                if (bloons_folder.GetChild(i).GetComponent<EnnemiScript>().played_this_turn == false)
                {
                    searchmember = bloons_folder.GetChild(i).gameObject;
                }
            }
        }

        if (set) 
        {
            return Memberturn = searchmember;
        }
        else
        {
            return searchmember;
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
            case 7:
                SpawnEntity("Bloon", Vector2.zero, 1);
                SpawnEntity("Bloon", Vector2.zero, 1);
                SpawnEntity("Bloon", Vector2.zero, 1);
                SpawnEntity("Bloon", Vector2.zero, 1);
                break;
            case 8:
                SpawnEntity("Bloon", Vector2.zero, 3);
                SpawnEntity("Bloon", Vector2.zero, 1);
                SpawnEntity("Bloon", Vector2.zero, 2);
                break;
            default:
                int redistribute_health = wave;
                while (redistribute_health > 0)
                {
                    int spawn_bloon_health = 0;
                    while (redistribute_health > 0 && spawn_bloon_health < 5)
                    {
                        ++spawn_bloon_health;
                        --redistribute_health;
                    }
                    SpawnEntity("Bloon", Vector2.zero, spawn_bloon_health);
                }
                break;
        }
    }

    private void RefillMagicShields()
    {
        for (int i = 0; i < monkes_folder.childCount; ++i)
        {
            monkes_folder.GetChild(i).GetComponent<HealthComponent>().GiveMagicShield(1);
        }
    }

    private void Update()
    {
        UpdateHUD();
    }

    private void FixedUpdate()
    {
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
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, camera_normal_zoom, 5f * Time.fixedDeltaTime);
        }
        else
        {
            float distance = (following.transform.position - following2.transform.position).magnitude;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, distance/2.25f, 5f * Time.fixedDeltaTime);
            cam.transform.position = (following.transform.position + following2.transform.position) / 2;
            cam.transform.position += new Vector3(0, 0, -cam.transform.position.z + camZ);
        }
        cam.orthographicSize = Mathf.Max(cam.orthographicSize, camera_normal_zoom);
    }
}
