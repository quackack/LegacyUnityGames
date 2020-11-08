using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
	public Texture2D Retical;
	bool PressLast;
	public GameObject LightningBase;
	public AudioSource shotSound;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit heart;
		if (Input.GetButtonDown("Fire1") || (Input.GetAxis("FireTrigger") != 0 && !PressLast)) {
			if (Physics.Raycast(transform.position, transform.forward, out heart)) {
				Heart status = heart.transform.GetComponent<Heart>();
				if (status != null) {
					if (status.Vuln == true) {
						status.Die ();
						Points.Score += 1;
					}
					else {
						Instantiate(heart.transform.gameObject, heart.transform.position+ Vector3.up*2, heart.transform.rotation);
						Points.Score -= 4;
					}
				}
				CreateLightning(heart.point);
			}
			else {
				CreateLightning(transform.forward*1000);
			}
			shotSound.Play();

		}

		if (Input.GetAxis("FireTrigger") != 0) {
			PressLast = true;
		}
		else {
			PressLast = false;
		}
	}
	
	void OnGUI () {
		float ReticalSize = Mathf.Min(Screen.height,Screen.width)*.05f;
		GUI.Label(new Rect(.5f*(Screen.width - ReticalSize), .5f*(Screen.height-ReticalSize), ReticalSize, ReticalSize), Retical);
	}
	
	void CreateLightning(Vector3 impact) {
		
		GameObject lightning = Instantiate(LightningBase, impact*.5f, transform.rotation) as GameObject;
		lightning.transform.localScale = new Vector3(.2f, .2f,impact.magnitude/5);
	}
}
