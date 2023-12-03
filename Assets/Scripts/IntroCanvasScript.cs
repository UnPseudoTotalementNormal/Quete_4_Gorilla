using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroCanvasScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI fulltitle;
    [SerializeField] private Image panel;
    [SerializeField] private Transform monkeintro;
    [SerializeField] private Transform bloonintro;

    public bool finished_animation = false;

    private int part = 1;

    private void Start()
    {
        panel.gameObject.SetActive(true);
        title.color -= new Color(0, 0, 0, 1);
        fulltitle.color -= new Color(0, 0, 0, 1);
    }
    private void Update()
    {

        if (finished_animation)
        {
            switch (part)
            {
                case 1:
                    if (title.color.a < 1)
                    {
                        title.color += new Color(0, 0, 0, 1f * Time.deltaTime);
                    }
                    else
                    {
                        part = 2;
                    }
                    break;
                case 2:
                    monkeintro.transform.position = new Vector3(Mathf.Lerp(monkeintro.transform.position.x, 15, 3 * Time.deltaTime), 0, 0);
                    bloonintro.transform.position = new Vector3(Mathf.Lerp(bloonintro.transform.position.x, -15, 3 * Time.deltaTime), 0, 0);
                    if (monkeintro.transform.position.x > 14)
                    {
                        part = 3;
                    }
                    break;
                case 3:
                    title.transform.position = new Vector3(title.transform.position.x, Mathf.Lerp(title.transform.position.y, fulltitle.transform.position.y, 4 * Time.deltaTime));
                    if (Mathf.Abs(title.transform.position.y - fulltitle.transform.position.y) < 0.03f)
                    {
                        panel.color -= new Color(0, 0, 0, 1f * Time.deltaTime);
                        fulltitle.color += new Color(0, 0, 0, 1f * Time.deltaTime);
                    }
                    break;
            }
        }
    }
}
