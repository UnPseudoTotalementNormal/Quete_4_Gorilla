using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    [SerializeField] int dir = 1;

    private int part = 1;

    [SerializeField] private float first_distance = 1;
    [SerializeField] private float second_distance = 1;
    [SerializeField] private float third_distance = 1;

    [SerializeField] private IntroCanvasScript introcanvas;

    private void Start()
    {
        StartCoroutine(intro());
    }

    private void FixedUpdate()
    {
        switch (part)
        {
            case 1:
                transform.position += new Vector3(20 * dir * Time.fixedDeltaTime, 0, 0);
                if (Mathf.Abs(transform.position.x) <=  first_distance)
                {
                    part = 2;
                }
                break;
            case 2:
                transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.localEulerAngles.z, 25 * dir, 3f * Time.fixedDeltaTime));
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, second_distance * -dir, 4f * Time.fixedDeltaTime), 0, 0);
                if (Mathf.Abs(transform.position.x) >= second_distance/1.1) 
                {
                    part = 3;
                }
                break;
            case 3:
                transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.localEulerAngles.z, -25 * dir, 6f * Time.fixedDeltaTime));
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, third_distance * -dir, 4f * Time.fixedDeltaTime), 0, 0);
                if (Mathf.Abs(transform.position.x) <= third_distance * 1.15)
                {
                    part = 4;
                    introcanvas.finished_animation = true;
                }
                break;
        }
    }

    private IEnumerator intro()
    {
        yield return new WaitForSeconds(2);
        //SceneManager.LoadSceneAsync("Scenes/MainMenu");
        yield return null;
    }
}