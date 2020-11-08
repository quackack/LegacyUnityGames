using UnityEngine;
using System.Collections;

public class GiveParticleTexture : MonoBehaviour {
	public Material theSystem;
	public Color X;
	public Color Y;
	public Color Z;
	public float Roughness = 0.25f;
	public float Quantity = 1;
	public int Detail = 512;
	public int Seed = 1;

	public Texture JustMade;
	// Use this for initialization
	void Start () {
		JustMade = TextureGenerator.getParticle(X, Y, Z, Roughness, Quantity, Detail, Seed);
		theSystem.SetTexture("_MainTex", JustMade);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
