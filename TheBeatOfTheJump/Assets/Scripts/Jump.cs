using UnityEngine;
using System.Collections;

public class Jump : MonoBehaviour {
	public float HeartRate;
	public bool SetPeriod;

	public Animation model;

	
	// Use this for initialization
	void Start () {
		SetPeriod = false;
		rigidbody.velocity = Vector3.right*4;
	}
	
	// Update is called once per frame
	void Update () {
		//HeartRate += Input.GetAxis("Horizontal")*Time.deltaTime;
		HeartRate = 1-Input.acceleration.y*2;
		HeartRate = Mathf.Clamp(HeartRate, 0.35f, 2);
		RaycastHit Ground;
		if (Physics.Raycast(transform.position, -transform.up, out Ground, 1.1f)) {
			rigidbody.velocity = new Vector3(rigidbody.velocity.x, Physics.gravity.magnitude*HeartRate/2, rigidbody.velocity.z);
			SetPeriod = true;
			Ground.transform.GetComponent<KillSelf>().AddPoint();
			model.Play();
			model["idle"].speed = 12/HeartRate;
		}
		else {
			SetPeriod = false;
		}
		
		if (transform.position.y < -20) {
			Application.LoadLevel("Death");
		}
	}
}
