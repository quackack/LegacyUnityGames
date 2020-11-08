using UnityEngine;
using System.Collections;

public class KillSelf : MonoBehaviour {
	public Transform Player;
	public float ViewRange;
	bool Scored = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Player.position.x - ViewRange > transform.position.x + transform.localScale.x*.5f) {
			Destroy(this.gameObject);
		}
	}
	public void AddPoint () {
		if (!Scored) {
			Scored = true;
			Score.Points+=1;
		}
	}
}
