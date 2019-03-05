using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SCR_MenuController : SCR_InputManager
{
    [SerializeField, Tooltip("Reference to the tutorial video player.")]
    private VideoPlayer tutorialVideoPlayer;

    [SerializeField, Tooltip("Reference to the main menu.")]
    private GameObject mainMenu;

    private SCR_InputAxis_1D escape;
    private bool inTutorial = false;

    private void Awake()
    {
        InitializeLists();
    }

    private void Start()
    {
        if (tutorialVideoPlayer)
        {
            tutorialVideoPlayer.enabled = false;
        }

        escape = BindInputAxis1D("Cancel", 0.0f);
    }

    private void Update()
    {
        UpdateInputAxises();
        if (escape.WasPressed)
        {
            if (inTutorial)
            {
                CancelInvoke("EndTutorial");
                EndTutorial();
            }
            else
            {
                Application.Quit();
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SCN_Main");

        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayTutorial()
    {
        tutorialVideoPlayer.enabled = true;
        tutorialVideoPlayer.Play();
        mainMenu.SetActive(false);
        Invoke("EndTutorial", 32.0f);
        inTutorial = true;
    }

    private void EndTutorial()
    {
        mainMenu.SetActive(true);
        tutorialVideoPlayer.Stop();
        tutorialVideoPlayer.time = 0.0f;
        tutorialVideoPlayer.enabled = false;
        inTutorial = false;
    }

}
