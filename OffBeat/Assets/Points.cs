using UnityEngine;
using System.Collections;

public class Points : MonoBehaviour {
	static public int Score = 0;
	static public float Lived = 0;
	static public bool JustStart = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			Score = 0;
			Lived = 0;
			JustStart = false;
			Application.LoadLevel("Test");
		}
	}
	
	void OnGUI () {
		string Message;
		string Invite;
		if (JustStart) {
			Message = "Welcome to off beat, where you keep beating hearts off by hitting them off beat";
			Invite = "See how long you can Last?";
		}
		else {
			Message = "You have " + Score + " points from destroying hearts, and have survive the game for " + Lived + " Seconds.";
			Invite = "Try Again?";
		}
		Rect StartLevel = new Rect(Screen.width*.1f, Screen.height*.1f, Screen.width*.8f, Screen.height*.8f);
		GUI.Label (StartLevel, Message);
		if (GUI.Button(new Rect(Screen.width*.4f, Screen.height*.8f, Screen.width*.2f, Screen.height*.2f), Invite)) {
			Score = 0;
			Lived = 0;
			JustStart = false;
			Application.LoadLevel("Test");
		}
		GUI.Label(new Rect(0, Screen.height*.9f, Screen.width, Screen.height*.1f), "This Song written by Kevin Macleod of Incompetech");
	}
}
