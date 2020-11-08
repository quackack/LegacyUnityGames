using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
	public static int Points;
	static bool FirstPlay = true;
	public string[] Statements;
	public string[] Clauses;
	int Statement;
	int Clause;
	// Use this for initialization
	void Start () {
		Statement = Random.Range(0, Statements.Length);
		Clause = Random.Range(0, Clauses.Length);
	}
	
	// Update is called once per frame
	void Update () {
		 if (Input.GetButtonDown("Fire1")) {
			Points = 0;
			FirstPlay = false;
			Application.LoadLevel("Level");
		}
	}
	
	void OnGUI () {
		string PointMessage;
		string ButtonMessage;
		if (FirstPlay) {
			Points = 0;
			PointMessage = "Welcome to the beat of the jump, "+ Clauses[Clause] ;
			ButtonMessage = "Begin " + Statements[Statement];
		}
		else {
			PointMessage= "You Jumped to " + Points.ToString() + " holes, "+ Clauses[Clause];
			ButtonMessage = "Restart " + Statements[Statement];
		}
		GUI.Label(new Rect(Screen.width*.2f, Screen.height*.2f, Screen.width*.6f, Screen.height*.6f), PointMessage);
		if (GUI.Button(new Rect(Screen.width*.4f, Screen.height*.8f, Screen.width*.2f, Screen.height*.2f), ButtonMessage)) {

			FirstPlay = false;
			Application.LoadLevel("Level");
		}
	}
}