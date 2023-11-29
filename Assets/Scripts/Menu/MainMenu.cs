using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 165;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Scenes/Level1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
