using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isPaused = false;

    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject gameOverScreen;

    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

    }

    public bool IsPaused()
    {
        return isPaused;
    }

    void Update()
    {
        if (!gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused)
                {
                    PauseGame();
                }
                else
                {
                    UnpauseGame();
                }
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowGameOver()
    {
        UnlockCursor();

        gameOver = true;
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void Restart()
    {
        gameOver = false;
        gameOverScreen.SetActive(false);
        UnpauseGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PauseGame()
    {
        UnlockCursor();
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void UnpauseGame()
    {
        LockCursor();
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
