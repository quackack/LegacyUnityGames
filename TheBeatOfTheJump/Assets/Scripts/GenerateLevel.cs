using UnityEngine;
using System.Collections;

public class GenerateLevel : MonoBehaviour {
	public Transform player;
	public float ViewRange;
	public float GapAllow;
	public float MinLand = 5;
	public float MaxLand = 20;
	public float Ground = -1.5f;
	public float LastLand;
	public GameObject[] land;
	// Use this for initialization
	void Start () {
		GameObject ground = Instantiate(land[Random.Range(0, land.Length)], new Vector3(player.position.x, Ground - ViewRange, player.position.z), Quaternion.identity) as GameObject;
		ground.transform.localScale = new Vector3(ViewRange*2, ground.transform.localScale.y*ViewRange*2, ground.transform.localScale.z*ViewRange*2);
		ground.GetComponent<KillSelf>().Player = player;
		LastLand = ViewRange;
	}
	
	// Update is called once per frame
	void Update () {
		if (player.position.x + ViewRange > LastLand+GapAllow) {
			float Size = Random.Range(MinLand, MaxLand);
			GameObject ground = Instantiate(land[Random.Range(0, land.Length)], new Vector3(LastLand+GapAllow +Size*.5f, Ground- Size*.5f, 0), Quaternion.identity) as GameObject;
			ground.transform.localScale = new Vector3(Size, Size, Size);
			LastLand = ground.transform.position.x+ground.transform.localScale.x*.5f;
			ground.GetComponent<KillSelf>().Player = player;
		}
		else {
			float Dist = Random.Range(0, GapAllow);
			if (Dist > LastLand) {
				float Size = Random.Range(MinLand, MaxLand);
				GameObject ground = Instantiate(land[Random.Range(0, land.Length)], new Vector3(LastLand+GapAllow +Size*.5f, Ground - Size*.5f, 0), Quaternion.identity) as GameObject;
				ground.transform.localScale = new Vector3(Size, Size, Size);
				ground.GetComponent<KillSelf>().Player = player;
				LastLand = ground.transform.position.x+ground.transform.localScale.x*.5f;
			}
		}

	
	}
}
