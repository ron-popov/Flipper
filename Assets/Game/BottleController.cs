#region Using Directives
	using UnityEngine;
	using UnityEngine.UI;

	using System;
	using System.IO;
	using System.Net;
	using System.Collections;
#endregion



public class BottleController : MonoBehaviour 
{
	#region Private Variables
		private Rigidbody2D r = null; //The bottle's rigidbody.
		private GameObject cam = null; //The game scene's camera.
		private GameObject bottle = null; //The bottle game object.
		private GameObject loseScreen = null; //The lose screen
		private GameObject floor = null; //The floor's game object.
		private Vector2 startingPoint; //The location of the mouse cursor at the moment the player first clicked the mouse.
		private Vector2 throwForce; //The location of the mouse cursor once the player released the mouse.
		private Text text; //The score text component.
		private Text header; //The main text component.
		private string userName; //The name of the player.
		private double startPressTime = 0; //The moment the user started holding the bottle.
		private float velocityMultiplier = 0.2f;		
		private int score = 0; //The player's score
		private bool wasMousePressed = false; //Whether the player is currently holding the mouse button to send the bottle.
		private bool wasBottleSent = false; //Whether the bottle has been already.
		private bool isGameOver = false; //Whether the game is over or not.
		private bool hasName; //Whether the player has a real name.
		private float rotationBefore; //The rotation of the bottle before the flip.
    #endregion

	#region MonoBehaviour Methods

		/* Called once at the beginning */
		void Start () 
		{
			score = 0;
			text = GameObject.Find("Canvas/ScoreCounter").GetComponent<Text>();
			text.text = FixFontString("Score : 0");
			throwForce = new Vector2(0 , 0);
			bottle = GameObject.Find("Bottle");
			r = bottle.GetComponent<Rigidbody2D>();		
			loseScreen = GameObject.Find("Canvas/LoseScreen");
			loseScreen.SetActive(false);
			r.transform.position = new Vector3(0 , -6 , 0);
			isGameOver = false;
			cam = GameObject.Find("Camera");
			floor = GameObject.Find("Floor");
			r.centerOfMass = new Vector2(0 , -2.5f);
			header = GameObject.Find("Canvas/Data").GetComponent<Text>();
			header.text = "";
			hasName = PlayerPrefs.HasKey("name") && PlayerPrefs.GetString("name").Length != 0;
			if(hasName)
				userName = PlayerPrefs.GetString("name");			
			else
				userName = "";

			if(!PlayerPrefs.HasKey("sensetivity"))
				velocityMultiplier = 0.2f;
			else
			{
				velocityMultiplier = PlayerPrefs.GetFloat("sensetivity");
			}

			r.mass = 1f;
			r.angularDrag = 3f;
			r.gravityScale = 5f;

		}


		/* Called every frame */
		void Update () 
		{
			if(Input.GetMouseButtonDown(0) && !wasBottleSent && !isGameOver)
			{
				startingPoint = new Vector2(Input.mousePosition.x , Input.mousePosition.y);
				wasMousePressed = true;
				startPressTime = DateTime.Now.Millisecond;
			}
			else if(Input.GetMouseButtonUp(0) && wasMousePressed && !wasBottleSent && !isGameOver)
			{
				double throwTime = (DateTime.Now.Millisecond - startPressTime)/1000;
				/* Calculating the force the player applied to the bottle */
				throwForce = new Vector2(((Input.mousePosition.x - startingPoint.x) / Screen.dpi) / float.Parse(throwTime.ToString()) , ((Input.mousePosition.y - startingPoint.y) / Screen.dpi) / float.Parse(throwTime.ToString()));

				/* The force applied to the bottle */
				float forceY = throwForce.y * velocityMultiplier;
				float forceX = throwForce.x * velocityMultiplier;

				/* Using the data collected to throw the bottle */
				rotationBefore = r.rotation;
				r.AddForceAtPosition(new Vector2(forceY , forceX) , startingPoint , ForceMode2D.Force);
				wasBottleSent = true;
				wasMousePressed = false;
			}
		}

	#endregion
		
	#region Flip Detection

		/* Successful flip */
		IEnumerator YesFlip()
		{
			yield return new WaitForSeconds(0.5f);
			text.text = FixFontString("Score : " + score);
			MoveSmoothly(cam , new Vector2(r.transform.position.x , cam.transform.position.z) , 1f);
		}

		/* Failed flip */
		IEnumerator NoFlip()
		{
			yield return new WaitForSeconds(0.5f);
			loseScreen.SetActive(true);
			floor.SetActive(false);
			bottle.SetActive(false);
		}

		/* Every frame there is a collision */
		void OnCollisionStay2D(Collision2D col)
		{
			//TODO: Will count as collision even if there was no rotation.
			if(col.contacts.Length == 2)
			{
				if(RegulateValue(col.contacts[0].point.y) == RegulateValue(col.contacts[1].point.y))
				{
					float len = Math.Abs(RegulateValue(col.contacts[0].point.x - col.contacts[1].point.x));
					if(len == 3 && RegulateValue(r.velocity.x) == 0 && RegulateValue(r.velocity.y) == 0 && RegulateValue(r.angularVelocity) == 0 && wasBottleSent && !isGameOver && Math.Abs(r.rotation - rotationBefore) > 90)
					{
						score++;
						wasBottleSent = false;					
						StartCoroutine(YesFlip());
					}
					else if (len == 6 && RegulateValue(r.velocity.x) == 0 && RegulateValue(r.velocity.y) == 0 && RegulateValue(r.angularVelocity) == 0 && wasBottleSent && !isGameOver)
					{
						isGameOver = true;
						StartCoroutine(NoFlip());

						//API KEY : "EagerBeautifulPanda"

						if(PlayerPrefs.GetInt("SendScore") == 1)
						{
							if(userName == "")
							{
								this.header.text = "In order to send your score , please set a name is the settings"; 
							}
							else
							{
								
								SendScoreThread(userName , score);
							}
						}					
						
					}
					else if(len == 0 && RegulateValue(r.velocity.x) == 0 && RegulateValue(r.velocity.y) == 0 && RegulateValue(r.angularVelocity) == 0 && wasBottleSent && !isGameOver)
					{
						score =+ 5;
						wasBottleSent = false;					
						StartCoroutine(YesFlip());
					}
					else if(len == 3 && RegulateValue(r.velocity.x) == 0 && RegulateValue(r.velocity.y) == 0 && RegulateValue(r.angularVelocity) == 0 && wasBottleSent && !isGameOver && Math.Abs(r.rotation - rotationBefore) < 10)
					{
						wasBottleSent = false;
					}
				}
			}
		}

	#endregion

	#region Assist Methods
		
		/* Moves the given object to the given location smoothly */
		private void MoveSmoothly(GameObject g , Vector2 loc , float time)
		{
			StartCoroutine(SmoothMovement(g , loc , time));
		}

		IEnumerator SmoothMovement(GameObject g , Vector2 loc , float time)
		{
			float d = loc.x - g.transform.position.x;
			float t = 0f;
			float initialX = g.transform.position.x;
			float initialVelocity = d*2/time;

			//The graph of the camera's location
			Func<float, float> f = new Func<float, float>( x => 
			{ 
				//return -1*((x - d/2).Square()) + (d/2).Square();
				return initialVelocity*x + (-(initialVelocity/time) * x * x)/2;
			});

			while(t < time)
			{
				t += 0.01f;
				g.transform.position = new Vector3(initialX + f.Invoke(t) , g.transform.position.y , g.transform.position.z);
				yield return new WaitForSeconds(0.01f);
			}
		}
		
		/* Return the given angle rounded to an even number and in the range between 0 and 360 degrees */
		float RegulateAngle(float angle)
		{
			float returnValue = (float)(Math.Round(Double.Parse(angle.ToString()) , MidpointRounding.ToEven));
			while(returnValue <= 0)
			{
				returnValue += 360;
			}
			while(returnValue >= 360 )
			{
				returnValue -= 360;
			}
			return returnValue;
		}

		/* Rounds a number */
		float RegulateValue(float f)
		{
			return (float)(Math.Round(Double.Parse(f.ToString()) , MidpointRounding.ToEven));
		}

		/* Fixes a string because of a stupid font design issue */
		string FixFontString(string num)
		{
			string returnValue = "";
			for(int i = 0 ; i < num.Length ; i++)
			{
				char c = num[i];
				if(c <= '9' && c >= '1')
					c--;
				else if (c == '0')
					c = '9';

				returnValue += c;
			}

			return returnValue;
		}

	#endregion

	#region Sending Score
		/* Sends the score to the server IN THE BACKGROUND */
		public void SendScoreThread(string scopeName , int score)
		{
			string url = ("http://dollarone.games/elympics/submitHighscore?key=EagerBeautifulPanda&name=" + scopeName + "&score=" + score);
			WWW www = new WWW(url);
			StartCoroutine(WaitForRequest(www));
				
		}

		IEnumerator WaitForRequest(WWW www)
		{
			yield return www;
	
			// check for errors
			if (www.error == null)
			{
				if(www.text == "OK")
				{
					Debug.Log("All good");
				}
				else
				{
					Debug.Log("WWW Error: "+ www.error);					
				}
			} else {
				Debug.Log("WWW Error: "+ www.error);
			}    
		}

		/* Sends a request to the given url */
		string SendHttp(string url)
		{
			// Create a request for the URL.   
            WebRequest request = WebRequest.Create (url);  
            request.Credentials = CredentialCache.DefaultCredentials;
			request.Timeout = 5000;
            WebResponse response = request.GetResponse();  
            Stream dataStream = response.GetResponseStream ();  
            StreamReader reader = new StreamReader (dataStream);
			string responseFromServer = reader.ReadLine();
            reader.Close ();  
            response.Close ();
			return responseFromServer;
		}
	#endregion
}