using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public float FloorHeight(float X, float Z)
	{
		return X*Z;
	}
}
