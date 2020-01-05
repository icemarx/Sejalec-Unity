using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool _paused = false;

    public GameObject pauseMenu;
    //public GameObject winMsg;

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!_paused && Time.timeScale > 0.5f)
            {
                Pause();
            }
            else if (_paused)
            {
                Unpause();
            }
        }
    }

    public bool GetIsPaused()
    {
        return _paused;
    }

    public void Pause()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _paused = true;
        pauseMenu.SetActive(true);
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        _paused = false;
    }

    public void WinMsg()
    {
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main");
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

}
