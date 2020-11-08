using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {
	public Transform Player;
	public Jump jump;
	public Vector3 AxisSensitivity;
	public Vector3 Init;
	public AudioClip beat;
	public AudioSource sound;
	// Use this for initialization
	void Start () {
		Init = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Init + new Vector3(Player.position.x*AxisSensitivity.x, Player.position.y*AxisSensitivity.y, Player.position.z*AxisSensitivity.z);
		transform.LookAt(new Vector3(transform.position.x, Player.position.y, Player.position.z));
		if (jump.SetPeriod) {
			audio.Play();
			audio.pitch = 1/jump.HeartRate;
		}
	}
}
