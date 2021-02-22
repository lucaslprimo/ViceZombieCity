using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isPaused = false;

    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject gameOverScreen;

    [SerializeField]
    private FPController player;

    [SerializeField]
    private Text gameTimeLabel;

    [SerializeField]
    private Animator doorAnimator;

    private float gameTime;

    private bool gameOver = false;

    private AudioSource musicPlayer;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        musicPlayer = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        gameTime = 0;
        musicPlayer.Play();
        Invoke("OpenDoor", 2f);
        StartCoroutine(StartTimer());
        player.LockPlayer(false);
    }

    private void OpenDoor()
    {
        doorAnimator.SetTrigger("open");
    }

    IEnumerator StartTimer()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(1);
            gameTime++;
            UpdateGameTimeUI();
        }
    }

    private void UpdateGameTimeUI()
    {
        gameTimeLabel.text = TimeSpan.FromSeconds(gameTime).ToString("mm\\:ss");
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
        musicPlayer.Pause();
        UnlockCursor();
        player.LockPlayer(true);
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
        UnpauseGame();
        gameOver = false;
        gameOverScreen.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PauseGame()
    {
        musicPlayer.Pause();
        UnlockCursor();
        player.LockPlayer(true);
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void UnpauseGame()
    {
        if(!gameOver)
            musicPlayer.Play();
        LockCursor();
        player.LockPlayer(false);
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
