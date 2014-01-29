using UnityEngine;
using System.Collections;

public class SmokeController : MonoBehaviour {
	
	public bool play;
	public float spreadFactor;
	private bool playing;
	private float lifeTimeDefault;
	
	void Awake () {
		play = true;
		playing = false;
		lifeTimeDefault = 1;
		spreadFactor = 1.0f;
		particleSystem.Stop();
		particleSystem.startSize = 2.0f;
		particleSystem.startSpeed = 0.2f;
		particleSystem.emissionRate = 50;
		particleSystem.startLifetime = lifeTimeDefault;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
			
		// Starts smoke emission
		if (play && !playing) {
			particleSystem.startLifetime = lifeTimeDefault;
			particleSystem.Play();
			playing = true;
			Debug.Log("Rauchentwicklung gestartet");
		}
		
		// Stops smoke emission
		if (!play && playing) {
			particleSystem.Stop();
			playing = false;
			Debug.Log("Rauchentwicklung gestoppt");
		}
		
		// Increases startLifetime depending on duration of smoke emission
		if (particleSystem.isPlaying) {
			particleSystem.startLifetime += (float) Time.deltaTime * spreadFactor;
		}
			
		
		// Veraendern von groesse, geschwindigkeit und lebensdauer
		if (Input.GetKeyDown(KeyCode.F1))
			particleSystem.startSize--;
		if (Input.GetKeyDown(KeyCode.F2))
			particleSystem.startSize++;
		if (Input.GetKeyDown(KeyCode.F3))
			particleSystem.startSpeed--;
		if (Input.GetKeyDown(KeyCode.F4))
			particleSystem.startSpeed++;	
		if (Input.GetKeyDown(KeyCode.F5))
			particleSystem.startLifetime--;
		if (Input.GetKeyDown(KeyCode.F6))
			particleSystem.startLifetime++;
	}
}
