#pragma strict

// Hannes Helmholz
//
// 


private class TriggerPanel extends TriggerAbstract {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField private var ObjectDescriptionOff:String;
	
	private var ObjectDescriptionOn:String; 
	private var State:boolean = false;
	private var Buttons:TriggerPanelButton[];
	private var Manager:TriggerPanelManager;
	// ============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		super.Awake();
		
		Buttons = gameObject.GetComponentsInChildren.<TriggerPanelButton>();
		Manager = GameObject.FindObjectOfType(TriggerPanelManager);
		
		ObjectDescriptionOn = ObjectDescription;
	}
	
	
	function OnMouseEnter() {
		if (State)
			NetView.RPC("RPCSetObjectDescription", RPCMode.AllBuffered, ObjectDescriptionOff);
		else
			NetView.RPC("RPCSetObjectDescription", RPCMode.AllBuffered, ObjectDescriptionOn);
		
		super.OnMouseEnter();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function SetState(state:boolean) {
		State = state;
			
		super.DoTrigger();
	}
	
	
	protected function DoTrigger() {
		Manager.SetState(!State);
		
		// StatusLast should not be reset on server
		// because focus still on FirePort but Highlighting should be off during measurement
		var last:boolean = StatusLast;
		NetView.RPC("RPCSetStatus", RPCMode.AllBuffered, false);
		StatusLast = last;
	}
	
	
	private function TriggerButtons() {
		for (var button:TriggerPanelButton in Buttons)
			button.Trigger();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCDoTrigger() {
		super.RPCDoTrigger();
		
		TriggerButtons();
	}
	
	
	@RPC
	function RPCSetObjectDescription(text:String) {
		ObjectDescription = text;
	}
	// =============================================================================
}