using UnityEngine;
using System.Collections;

public class BallOfLight : MonoBehaviour {
	public Vector3 SpinRate;
	public float DeathHeight;
	public Color LightCol1;
	public Color LightCol2;
	public ParticleSystem PartSyst;
	// Use this for initialization
	void Start () {
		Color ThisBall = Color.Lerp(LightCol1, LightCol2, Random.Range(0f, 1f));
		this.light.color = ThisBall;
		PartSyst.startColor = ThisBall;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(SpinRate*Time.deltaTime);
		if (transform.position.y < DeathHeight) {
			Destroy(this.gameObject);
		}
	}
}
