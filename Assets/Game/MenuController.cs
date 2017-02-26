using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour 
{
	//Once the retry button is clicked in the retry menu
	public void OnRetryClick()
	{
		SceneManager.LoadScene("Game", LoadSceneMode.Single);
		GameObject.Find("Canvas/Data").GetComponent<Text>().text = "";
	}

	//Once the menu button is clicked in the retry menu
	public void OnMenuClick()
	{
		SceneManager.LoadScene("Menu", LoadSceneMode.Single);
	}
}
