
// Hannes Helmholz
//
// It calculates frames/second over each updateInterval, so the display does not keep changing wildly.
//
// It is also fairly accurate at very low FPS counts (<10). We do this not by simply counting frames per interval, but
// by accumulating FPS for each frame. This way we end up with correct overall FPS even if the interval renders something like 5.5 frames.

using UnityEngine;

public class FramesPerSecond : MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	[SerializeField]	private bool OnlyShowInDebugMode = true;
	[SerializeField]	private float UpdateInterval = 0.5f;
	
	private Config Config;
	private float Accum = 0.0f; // FPS accumulated over the interval
	private int Frames = 0; // Frames drawn over the interval
	private float TimeLeft; // Left time for current interval
	private string Text = "";
	// =============================================================================
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	void Start() {
		Config = GameObject.FindWithTag("Config").GetComponent<Config>();
		
	    TimeLeft = UpdateInterval;  
	}
	
	
	void Update() {
	    TimeLeft -= Time.deltaTime;
	    Accum += Time.timeScale / Time.deltaTime;
	    ++Frames;
	 
	    // Interval ended - update GUI text and start new interval
	    if (TimeLeft <= 0.0) {
	        // display one fractional digits (f1 format)
	        Text = (Accum / Frames).ToString("f1");
	        TimeLeft = UpdateInterval;
	        Accum = 0.0f;
	        Frames = 0;
	    }
	}
	
	
	void OnGUI(){
//dan		
//		if (OnlyShowInDebugMode && !Config.LogOnScreen)
//			return;
//	
//		GUI.skin = Config.InterfaceSkin;
//		
//		GUI.Label( new Rect(Screen.width - 110 + (30 * Config.OwnClientData.ParallaxOffsetDirection), 30, 40, 20), Text + " FPS", "Debug");
	}
	// =============================================================================
}