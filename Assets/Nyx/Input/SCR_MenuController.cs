using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_MenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SCN_Main");

        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
