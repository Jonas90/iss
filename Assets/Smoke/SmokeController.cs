using UnityEngine;
using System.Collections;

public class SmokeController : MonoBehaviour {
	
	public bool play;
	
	public float spreadFactor;
	private bool playing;
	private float lifeTimeDefault;
    private NetworkView NetView;
	
	void Awake () {
        NetView = networkView;
		//play = true;
		//playing = false;
		//lifeTimeDefault = 1;
		//spreadFactor = 1.0f;
		particleSystem.Stop();
		//particleSystem.startSize = 2.0;
		//particleSystem.startSpeed = 0.2f;
		//particleSystem.emissionRate = 50;
		particleSystem.startLifetime = lifeTimeDefault;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	private void changeState(bool play) {
		Debug.Log("changeState: " + play);
		if (play) {
			particleSystem.startLifetime = lifeTimeDefault;
			particleSystem.Play();
			Debug.Log("Rauchentwicklung gestartet");
		} else {
			particleSystem.Stop();
			Debug.Log("Rauchentwicklung gestoppt");
		}
		this.play = play;
		this.playing = play;
	}
	
	// Update is called once per frame
	void Update () {
			
		// Starts smoke emission
		if (play != playing) {
			Debug.Log("play != playing: " + play);
			if (Config.Instance.IsStandalone) {
				changeState(play);
			} else {
				Debug.Log("play != playing:NotStandalone");
				Debug.Log("NetView:" + NetView);
				Debug.Log("NetView2:" + networkView);
				networkView.RPC( "changeStateRPC", RPCMode.All, play );
				Debug.Log("play != playing:NotStandalone2");
			}
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
	
	[RPC]
	public void changeStateRPC(bool play) {
		Debug.Log("changeStateRPC");
		changeState(play);
	}
}
