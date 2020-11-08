using UnityEngine;
using System.Collections;

public class SeekPlayer : MonoBehaviour {
	public Transform player;
	public float TopSeedSqrd;
	public Vector3 Velocity;
	public float Acceleration;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 Accelerate = (player.position-transform.position).normalized*Time.deltaTime*Acceleration;
		Velocity = Velocity+Accelerate;
		if (Velocity.sqrMagnitude > TopSeedSqrd) {
			Velocity.Normalize();
		}
		transform.position+=Velocity*Time.deltaTime;
		transform.LookAt(player.position);
	}
}
