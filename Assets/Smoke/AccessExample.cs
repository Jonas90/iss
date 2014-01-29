using UnityEngine;
using System.Collections;

public class AccessExample : MonoBehaviour {

	GameObject smoke;
	SmokeController smokeController;
	
	// Use this for initialization
	void Start () {
		
		// Create a reference to smoke GameObject
		smoke = GameObject.Find("SmokeParticleSystem");
		
		// Create a reference to smokeController script
		smokeController = smoke.GetComponent<SmokeController>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown(KeyCode.Space)) {
			
			// Set smokeController.play to true activates smoke emission, false disable smoke emission
			smokeController.play = !smokeController.play;
		}
	}
}
