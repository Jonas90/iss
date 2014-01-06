#pragma strict

// Hannes Helmholz
//
// 


@script RequireComponent(NetworkView); // for RPC

private class TriggerPanelManager extends MonoBehaviour {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	private var NetView:NetworkView;
	private var State:boolean = false;
	private var Panels:TriggerPanel[];
	// ============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		NetView = networkView;
	
		Panels = gameObject.GetComponentsInChildren.<TriggerPanel>();
	}
	
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------

	public function SetState(state:boolean) {
		NetView.RPC("RPCSetState", RPCMode.AllBuffered, state);
	}
	
	
	private function SetPanels(state:boolean) {
		for (var panel:TriggerPanel in Panels)
			panel.SetState(state);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCSetState(state:boolean) {
		State = state;
		
		SetPanels(State);
	}
	// =============================================================================
}