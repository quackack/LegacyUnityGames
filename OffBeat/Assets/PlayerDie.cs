using UnityEngine;
using System.Collections;

public class PlayerDie : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnTriggerEnter () {
		Application.LoadLevel("Death");
	}
}
