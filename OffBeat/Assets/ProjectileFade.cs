using UnityEngine;
using System.Collections;

public class ProjectileFade : MonoBehaviour {
	public float life;
	float lived = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		lived += Time.deltaTime;
		Color matCol = renderer.material.GetColor("_TintColor");
		matCol.a = Mathf.Abs(lived/life-.5f);
		renderer.material.SetColor("_TintColor", matCol);
		if (lived > life) {
			Destroy(this.gameObject);
		}
	}
}
