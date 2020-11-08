using UnityEngine;
using System.Collections;

public class RotateBase : MonoBehaviour {
	public float MouseSens = 2;
	public float StickSens = 90;
	
	Gyroscope Gyro;
	// Use this for initialization
	void Start () {
		Gyro = Input.gyro;
		Gyro.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		//Vector3 Comp = Input.compass.rawVector;
		Vector3 Rot = Gyro.attitude.eulerAngles;
		Rot.y *= -1;
		transform.localRotation = Quaternion.Euler(Rot);
		//transform.localRotation = Quaternion.LookRotation(Comp);
		//transform.Rotate(0, MouseSens*Input.GetAxis("Mouse X") + StickSens*Input.GetAxis("Horizontal")*Time.deltaTime, 0, Space.World);
	}
}
