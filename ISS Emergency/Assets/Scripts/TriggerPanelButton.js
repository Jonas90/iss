#pragma strict

// Hannes Helmholz
//
// 


private class TriggerPanelButton extends MonoBehaviour {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField private var ColorOn:Color;
	
	private var Mat:Material;
	private var Audio:AudioSource;
	private var State:boolean = false;
	private var InitState:boolean = true;
	private var ColorOff:Color;
	// ============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		Mat = renderer.material;
		Audio = audio;
		
		ColorOff = Mat.color;
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function SetState(state:boolean) {
		if (state != State)
			Trigger();
	}
	
	
	public function Trigger() {
		State = !State;
		
		if (InitState) {
			//Mat.color = State ? ColorOn : ColorOff;
			Mat.color = ColorOn;
			InitState = false;
		}
		
		if (Audio) {
			if (State)
				Audio.Play();
			else
				Audio.Stop();
		}
	}
	// =============================================================================

}