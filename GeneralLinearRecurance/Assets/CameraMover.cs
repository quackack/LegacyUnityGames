using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
    public float rotationSensitivity = 3;
    public float moveSensitivityLog = 0;
    public float moveSensLowBound = -10;
    public float moveSensHiBound = 10;
	
	// Update is called once per frame
	void Update ()
    {
        moveSensitivityLog += Input.GetAxis("Mouse ScrollWheel");
        moveSensitivityLog = Mathf.Clamp(moveSensitivityLog, moveSensLowBound, moveSensHiBound);
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * Mathf.Exp(moveSensitivityLog));
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * rotationSensitivity, Space.Self);
    }
}
