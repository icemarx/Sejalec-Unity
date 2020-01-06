using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool _paused = false;

    public GameObject pauseMenu;
    //public GameObject winMsg;

    public Image scoreContainer;

    private float _timer = 0;
    private int timeInSeconds = 0;
    public Text timerText;

    public Text seedsText;
    public Text waterText;

    public GameObject voxleFarm;
    private int _groundBlocksCount = 0;
    public float winCondition = 0.6f;
    private int _grassyBlocksCount = 0;

    void Start()
    {
        _groundBlocksCount = voxleFarm.GetComponent<VoxleFarm>().GetGroundBlocksCount();
        SetScore(0);
    }

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

        if(!_paused)
        {
            SetTimer();
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

    public void SetTimer()
    {
        _timer += Time.deltaTime;

        int newTimeInSeconds = (int)Mathf.Floor(_timer);
        if(newTimeInSeconds == timeInSeconds)
        {
            return;
        }
        else
        {
            timeInSeconds = newTimeInSeconds;
        }

        int minutes = timeInSeconds / 60;
        int seconds = timeInSeconds % 60;

        string timeForDisplay = minutes.ToString();

        if(seconds<10)
        {
            timeForDisplay += ":0" + seconds;
        } 
        else
        {
            timeForDisplay += ":" + seconds;
        }


        timerText.text = timeForDisplay;
    }

    public void SetScore(int procentage)
    {
        float width = scoreContainer.GetComponent<RectTransform>().rect.width-4;
        var scoreRect = scoreContainer.transform.GetChild(0).gameObject.transform as RectTransform;

        float scoreWidth = width / 100f * procentage;

        scoreRect.sizeDelta = new Vector2(scoreWidth, scoreRect.sizeDelta.y);
    }

    public void AddToScore(int count)
    {
        int winCount = Mathf.CeilToInt(_groundBlocksCount * winCondition);
        _grassyBlocksCount += count;


        if(_grassyBlocksCount > winCount)
        {
            SetScore(100);
            //TODO win screen
            Debug.Log("You win");
        }
        else
        {
            SetScore(Convert.ToInt32((double)_grassyBlocksCount / winCount * 100));
        }
    }

    public void SetSeedsNumber(int seedsNum)
    {
        seedsText.text = seedsNum.ToString();
    }

    public void SetWaterNumber(int waterNum)
    {
        waterText.text = waterNum.ToString();
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
