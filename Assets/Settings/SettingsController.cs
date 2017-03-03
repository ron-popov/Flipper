using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsController : MonoBehaviour {

	//Text compoent.
	Text text;

	//Called on start.
	void Start()
	{
		text = GameObject.Find("Canvas/Text").GetComponent<Text>();
		text.text = "";
		if(PlayerPrefs.HasKey("name") && PlayerPrefs.GetString("name").Length > 0)
		{
			string name = PlayerPrefs.GetString("name");
			GameObject.Find("Canvas/InputField/Placeholder").GetComponent<Text>().text = name;
		}

		if(PlayerPrefs.HasKey("sensetivity"))
		{
			GameObject.Find("Canvas/Slider").GetComponent<Slider>().value = (PlayerPrefs.GetFloat("sensetivity") + 0.2f) / 0.4f;
		}
	}

	//Once save button is clicked.
	public void OnSaveClick()
	{
		#region Name Input
			string name = GameObject.Find("Canvas/InputField/Text").GetComponent<Text>().text;
			

			if(GameObject.Find("Canvas/Toggle").GetComponent<Toggle>().isOn)
			{
				PlayerPrefs.SetInt("SendScore" , 1);
				if(name.Length == 0)
				{
					if(PlayerPrefs.HasKey("name") && PlayerPrefs.GetString("name").Length > 0)
					{
						name = PlayerPrefs.GetString("name");					
					}
					else
					{
						text.text = "Name too short";
						return;					
					}

				}
				else if(name.Length > 32)
				{
					text.text = "Name is too long";
					return;
				}
			}
			else
			{
				PlayerPrefs.SetInt("SendScore" , 0);
			}
		#endregion

		#region Sensetivity Input
			float sensetivity = GameObject.Find("Canvas/Slider").GetComponent<Slider>().value;
			float finalSensetivity;
			if(sensetivity == 1f)
				finalSensetivity = 0.2f;
			else if(sensetivity == 2f)
				finalSensetivity = 0.6f;
			else if(sensetivity == 3f)
				finalSensetivity = 1f;
			else
				finalSensetivity = 0.6f;
		#endregion

		/* Saving the data */
		PlayerPrefs.SetString("name" , name);
		PlayerPrefs.SetFloat("sensetivity" , finalSensetivity);

		SceneManager.LoadScene("Menu" , LoadSceneMode.Single);		
	}


	//API KEY : "EagerBeautifulPanda"


	//Once menu button is clicked.
	public void OnMenuClick()
	{
		SceneManager.LoadScene("Menu" , LoadSceneMode.Single);
	}
}
