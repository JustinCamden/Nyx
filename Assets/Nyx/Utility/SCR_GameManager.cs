using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SCR_GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("Number of enemies to defeat before achieving victory.")]
    private int enemiesRemaining = 100;

    [SerializeField, Tooltip("Counter for number of enemies remaining.")]
    private Text enemiesRemainingCounter;

    [SerializeField, Tooltip("Player character reference.")]
    private SCR_PlayerController playerChar;

    [SerializeField, Tooltip("Spawners for standard enemies")]
    private SCR_EnemySpawner[] basicEnemySpawners;

    [SerializeField, Tooltip("Reference to player input.")]
    private SCR_PlayerActions playerActions;

    [SerializeField, Tooltip("Reference to the pause menu.")]
    private GameObject pauseMenu;

    [SerializeField, Tooltip("Reference to the end game message.")]
    private GameObject endGameMessage;

    [SerializeField, Tooltip("Reference to the end game text.")]
    private Text endGameText;

    [SerializeField, Tooltip("The maximum number of enemies at a time.")]
    private int maxEnemies = 25;

    [SerializeField, Tooltip("The minimum time between enemy spawns.")]
    private float minTimeBetweenSpawns = 0.5f;

    [SerializeField, Tooltip("The maximum time between enemy spawns.")]
    private float maxTimeBetweenSpawns = 3.0f;

    [SerializeField, Tooltip("The delay between the end of the game and returning to the menu.")]
    private float endGameDelay = 3.0f;

    // State variables
    private int numEnemies = 0;
    private bool enemyQueued = false;
    private bool paused = false;
    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!playerActions)
        {
            playerActions = SCR_PlayerActions.Instance;
        }

        enemyQueued = true;
        Invoke("SpawnEnemy", Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns));
        enemiesRemainingCounter.text = enemiesRemaining.ToString();
        pauseMenu.SetActive(false);
        endGameMessage.SetActive(false);

        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        // Handle pausing and un-pausing the game
        if (playerActions && playerActions.menu.WasPressed && !gameOver)
        {
            if (paused)
            {
                UnPause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        paused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void UnPause()
    {
        paused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void PlayerLose()
    {
        foreach (SCR_EnemyController_Standard enemyChar in FindObjectsOfType<SCR_EnemyController_Standard>())
        {
            enemyChar.Victory();
        }
        gameOver = true;
        endGameMessage.SetActive(true);
        endGameText.text = "You Lost";
        Invoke("ReturnToMenu", endGameDelay);
    }

    public void PlayerWin()
    {
        playerChar.GetComponent<SCR_RobotCharacter>().OnVictory();
        gameOver = true;
        endGameMessage.SetActive(true);
        endGameText.text = "You Win!";
        Invoke("ReturnToMenu", endGameDelay);
    }

    public void OnEnemyDeath()
    {
        numEnemies -= 1;
        enemiesRemainingCounter.text = (enemiesRemaining + numEnemies).ToString();
        if (enemiesRemaining <= 0 && numEnemies <= 0)
        {
            PlayerWin();
        }
        else if (!enemyQueued && enemiesRemaining > 0)
        {
            enemyQueued = true;
            Invoke("SpawnEnemy", Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns));
        }
    }

    public void SpawnEnemy()
    {
        int idx = Random.Range(0, basicEnemySpawners.Length);
        if (idx > basicEnemySpawners.Length - 1)
        {
            idx--;
        }
        GameObject newEnemy = basicEnemySpawners[idx].SpawnEnemy();
        newEnemy.GetComponent<SCR_Health>().onDeath += OnEnemyDeath;
        enemiesRemaining--;
        numEnemies++;
        if (enemiesRemaining <= 0  || numEnemies >= maxEnemies)
        {
            enemyQueued = false;
        }
        else
        {
            Invoke("SpawnEnemy", Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns));
        }
    }

    public void QuiteGame()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("SCN_MainMenu");
    }
}
