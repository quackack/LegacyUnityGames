using UnityEngine;
using System.Collections;

public class FreeCamera : MonoBehaviour {
	public float maxSpeed;
	public float minSpeed;
	public float speed;

	public float sens = 1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))*speed*Time.deltaTime);
		if (Input.GetButton("Fire1")) {
			speed = speed * Mathf.Exp(-Time.deltaTime);
		}
		if (Input.GetButton("Fire2")) {
			speed = speed * Mathf.Exp(Time.deltaTime);
		}
		speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

		transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"))*sens);
	}
}
