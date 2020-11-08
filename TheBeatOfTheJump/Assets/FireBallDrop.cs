using UnityEngine;
using System.Collections;

public class FireBallDrop : MonoBehaviour {
	public GameObject[] drops;
	public Vector3 InitialVel;
	public Vector3 MinArea;
	public Vector3 MaxArea;
	public float SpawnRate = 1;
	float TimeSinceSpawn = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		TimeSinceSpawn += Time.deltaTime;
		if (Random.Range(0, SpawnRate) < TimeSinceSpawn) {
			TimeSinceSpawn -= SpawnRate;
			Vector3 SpawnLoc = new Vector3(Random.Range(MinArea.x, MaxArea.x), Random.Range(MinArea.y, MaxArea.y), Random.Range(MinArea.z, MaxArea.z));
			GameObject Drip = Instantiate(drops[Random.Range(0, drops.Length)], transform.position+SpawnLoc, Quaternion.identity) as GameObject;
			Drip.rigidbody.velocity = InitialVel;
		}
	}
}
