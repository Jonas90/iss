#pragma strict

// Hannes Helmholz
//
// It calculates frames/second over each updateInterval, so the display does not keep changing wildly.
//
// It is also fairly accurate at very low FPS counts (<10). We do this not by simply counting frames per interval, but
// by accumulating FPS for each frame. This way we end up with correct overall FPS even if the interval renders something like 5.5 frames.


private class FramesPerSecond extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var OnlyShowInDebugMode:boolean = true;
	@SerializeField	private var UpdateInterval:float = 0.5;
	
	private var Config:ConfigClass;
	private var Accum:float = 0.0; // FPS accumulated over the interval
	private var Frames:int = 0; // Frames drawn over the interval
	private var TimeLeft:float; // Left time for current interval
	private var Text:String = "";
	// =============================================================================
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		
	    TimeLeft = UpdateInterval;  
	}
	
	
	function Update() {
	    TimeLeft -= Time.deltaTime;
	    Accum += Time.timeScale / Time.deltaTime;
	    ++Frames;
	 
	    // Interval ended - update GUI text and start new interval
	    if (TimeLeft <= 0.0) {
	        // display one fractional digits (f1 format)
	        Text = (Accum / Frames).ToString("f1");
	        TimeLeft = UpdateInterval;
	        Accum = 0.0;
	        Frames = 0;
	    }
	}
	
	
	function OnGUI(){
		if (OnlyShowInDebugMode && !Config.LogOnScreen)
			return;
	
		GUI.skin = Config.InterfaceSkin;
		
		GUI.Label(Rect(Screen.width - 110 + (30 * Config.OwnClientData.ParallaxOffsetDirection), 30, 40, 20), Text + " FPS", "Debug");
	}
	// =============================================================================
}