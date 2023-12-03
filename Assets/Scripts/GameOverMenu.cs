using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI wavetext;
    private void Update()
    {
        if (DataKeeper.wave_reached > 1)
        {
            wavetext.text = "You survived " + DataKeeper.wave_reached.ToString() + " Waves";
        }
        else
        {
            wavetext.text = "You survived " + DataKeeper.wave_reached.ToString() + " Wave";
        }
    }
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void Main_menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
