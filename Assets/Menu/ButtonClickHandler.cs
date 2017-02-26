using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClickHandler : MonoBehaviour 
{
	public GameObject credits = null;

	void Start()
	{
		credits = GameObject.Find("Canvas/Credits");
		credits.SetActive(false);
	}

	//Once the start button is clicked.
	public void OnStartClick()
	{
		SceneManager.LoadScene("Game", LoadSceneMode.Single);
	}

	//Once the settings button is clicked.
	public void OnSettingsClick()
	{
		//TODO: Finish the settings scene
		SceneManager.LoadScene("Settings" , LoadSceneMode.Single);
	}

	//Once the credits button is clicked.
	public void OnCreditsClick()
	{
		credits.SetActive(true);
		GameObject.Find("Canvas/Credits/Panel").GetComponent<RectTransform>().sizeDelta = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta;
	}

	//Once the close credits button is clicked.
	public void OnCloseCredits()
	{
		credits.SetActive(false);
	}

	//Once the exit button is clicked.
	public void OnExitClick()
	{
		Application.Quit();
	}

	
}
