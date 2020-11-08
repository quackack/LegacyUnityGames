using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Exit") || Input.GetKey(KeyCode.Escape)) {
			print ("Left");
			Application.Quit();
		}
	
	}
}
