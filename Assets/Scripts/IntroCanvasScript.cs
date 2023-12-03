using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroCanvasScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Image panel;
    [SerializeField] private Transform monkeintro;
    [SerializeField] private Transform bloonintro;

    public bool finished_animation = false;

    private bool already_started = false;

    private void Start()
    {
        panel.gameObject.SetActive(true);
        title.color = new Color(255, 255, 255, 0);
    }
    private void Update()
    {
        if (!already_started && finished_animation)
        {
            already_started = true;
            StartCoroutine(MenuAnimation());
        }
    }

    private IEnumerator MenuAnimation() 
    {
        while (true)
        {
            title.color += new Color(0, 0, 0, 1);
        }
        //gameObject.SetActive(false);
        yield return null;
    }
}
