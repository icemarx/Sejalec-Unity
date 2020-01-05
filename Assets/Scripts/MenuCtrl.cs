using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuCtrl : MonoBehaviour
{
	public GameObject Controls;
	public GameObject Credits;

	public void StartGame()
	{
		SceneManager.LoadScene("Main");
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
