using UnityEngine;
using System.Collections;

public class SpawnEnemies : MonoBehaviour {
	public GameObject[] Enemies;
	public float InitSpawnRate;
	public float RateAccel;
	public float DistSpawn;
	
	float SpawnRate;
	float Step;
	// Use this for initialization
	void Start () {
		Step = .01f;
	
	}
	
	// Update is called once per frame
	void Update () {
		Step += Time.deltaTime;
		Points.Lived += Time.deltaTime;
		SpawnRate = InitSpawnRate/Points.Lived;
		if (Step > SpawnRate) {
			Vector3 Location = new Vector3(Random.Range (-1f, 1f), Random.Range (-1f, 1f), Random.Range (-1f, 1f)).normalized*DistSpawn;
			if (Location == Vector3.zero) {
				Location = Vector3.forward*DistSpawn;
			}
			GameObject heart = Instantiate(Enemies[Random.Range(0, Enemies.Length)], Location, Quaternion.identity) as GameObject;
			SeekPlayer Thisheart = heart.GetComponent<SeekPlayer>();
			Vector3 RandDir = new Vector3(Random.Range (-1, 1), Random.Range (-1, 1), Random.Range (-1, 1)).normalized;
			Thisheart.Velocity = Vector3.Cross(Location,  RandDir)*5;
			Thisheart.player = transform;
			Step = 0;
		}
	
	}
}
