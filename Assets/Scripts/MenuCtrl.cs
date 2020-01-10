using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuCtrl : MonoBehaviour
{
	public GameObject Difficulty;
	public GameObject Controls;
	public GameObject Credits;

	public DifficultySettings difficultySettings;

	private void Awake()
	{
		DontDestroyOnLoad(difficultySettings);
	}

	public void StartGame(int spawnGhostSec, int holdSeeds, int holdWater)
	{
		DifficultySettings.SpawnGhostSec = spawnGhostSec;
		DifficultySettings.HoldSeeds = holdSeeds;
		DifficultySettings.HoldWater = holdWater;
		SceneManager.LoadScene("Main");
	}

	public void DifficultyOn()
	{
		Difficulty.SetActive(true);
	}

	public void DifficultyOff()
	{
		Difficulty.SetActive(false);
	}

	public void HandleEasy()
	{
		StartGame(50, 50, 25);
	}

	public void HandleMedium()
	{
		StartGame(40, 30, 15);
	}

	public void HandleHard()
	{
		StartGame(30, 20, 10);
	}

	public void ControlsOn()
	{
		Controls.SetActive(true);
	}

	public void ControlsOff()
	{
		Controls.SetActive(false);
	}

	public void CreditsOn()
	{
		Credits.SetActive(true);
	}

	public void CreditsOff()
	{
		Credits.SetActive(false);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
