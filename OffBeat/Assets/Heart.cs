using UnityEngine;
using System.Collections;

public class Heart : MonoBehaviour {
	public Texture2D Icon;
	public AudioSource Beat;
	public Renderer skin;
	public Color MyCol;
	public Color ItsCol;
	public float HeartTimeVuln;
	public float HeartTimeInVuln;
	public float VulnSize;
	public float InVulnSize;
	public bool Vuln;
	public bool Dead;
	public float DieTime;
	public ParticleSystem Halo;
	float Step = 0;
	float LiveSize = 1;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Step += Time.deltaTime;
		if (!Dead) {
			UpdateBeat();
		}
		else {
			if (Step > DieTime) {
				Destroy(this.gameObject);
			}
			float Size = transform.localScale.x - LiveSize*Time.deltaTime;
			transform.localScale = new Vector3(Size, Size, Size);
			skin.material.color = Color.Lerp(MyCol, ItsCol, Step/DieTime);
		}
	}
	
	void OnGUI () {
		Vector3 ScreenPos = Camera.mainCamera.WorldToScreenPoint(transform.position);
		if (ScreenPos.x < 0 || ScreenPos.x > Screen.width || ScreenPos.y < 0 || ScreenPos.y > Screen.height || ScreenPos.z < 0) {
			float Size = Mathf.Min (Screen.height, Screen.width)*.1f;
			ScreenPos.x = ScreenPos.x*Mathf.Sign(ScreenPos.z) - Screen.width*.5f;
			ScreenPos.y = Screen.height*.5f-ScreenPos.y*Mathf.Sign(ScreenPos.z);
			ScreenPos.z = 0;
			ScreenPos = ScreenPos.normalized*Mathf.Min (Screen.width, Screen.height)*.5f;
			ScreenPos.x = ScreenPos.x + Screen.width*.5f;
			ScreenPos.y = ScreenPos.y + Screen.height*.5f;
			Rect ScreenLoc = new Rect(ScreenPos.x - Size*.5f, ScreenPos.y - Size*.5f, Size, Size);
			GUI.Label(ScreenLoc, Icon);
		}
	}
	
	public void UpdateBeat () {
			if (Vuln) {
				if (Step > HeartTimeVuln) {
					Vuln = false;
					Step %= HeartTimeVuln;
					if (Beat != null) {
						Beat.Play();
					}
				}
			}
			else {
				if (Step > HeartTimeInVuln) {
					Vuln = true;
					Step %= HeartTimeInVuln;
				}
			}
			if (Vuln) {
				float Size = Mathf.Lerp (InVulnSize, VulnSize, Step/HeartTimeVuln);
				transform.localScale = new Vector3(Size, Size, Size);
			}
			else {
				float Size = Mathf.Lerp (VulnSize, InVulnSize, Step/HeartTimeInVuln);
				transform.localScale = new Vector3(Size, Size, Size);
			}
	}
	
	public void Die () {
		Destroy (this.collider);
		Step = 0;
		LiveSize = transform.localScale.x;
		Dead = true;
		Halo.emissionRate =0;
	}
	
	
}
