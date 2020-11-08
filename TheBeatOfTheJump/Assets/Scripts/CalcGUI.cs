using UnityEngine;
using System.Collections;

public class CalcGUI : MonoBehaviour {
	public Texture2D Bar;
	public Jump Beat;
	public Transform Location;
	public Rigidbody Vel;
	public float Ground =-1.5f;
	float PeriodBegin;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Beat.SetPeriod) {
			PeriodBegin = Location.position.x + .6f + Beat.HeartRate*Vel.velocity.x + .0625f*Beat.HeartRate*Beat.HeartRate;
		}
	}
	
	void OnGUI () {
		Vector2 Center;
		float point;
		Rect Rectangle;
		for (int i = -6; i < 7; i++) {
			point = PeriodBegin + Beat.HeartRate*Vel.velocity.x*i + .0625f*Beat.HeartRate*Beat.HeartRate*i*Mathf.Abs (i);
			Center = Camera.mainCamera.WorldToScreenPoint(new Vector3(point, Ground, 0));
			Rectangle = new Rect(Center.x-Screen.height*.03f, Screen.height*.97f-Center.y, Screen.height*.06f, Screen.height*.06f);
			GUI.Label(Rectangle, Bar);
		}
	}
}
