#pragma strict

// Hannes Helmholz
//
// 


@script RequireComponent(NetworkView) // for RPC

private class TriggerButtonSequence extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var RestartAfterFinish:boolean = true;
	@SerializeField	private var Buttons:TriggerButton[];
	
	private var Config:ConfigClass;
	private var NetView:NetworkView;
	private var Current:int = -1;
	private var Finished:boolean = false;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		
		NetView = networkView;
		
		for (var button:TriggerButton in Buttons) {
			button.SetAnimation(animation); // needs to be done
			button.enabled = false;
		}
		
		RPCSetCurrent(0);
		Buttons[Current].SetAsStartingPoint();
	}
	
	
	function Update() {
		if (!Config.IsServer)
			return;
			
		if (IsCurrentTriggered())
			SetAllCurrentSERVERONLY(Current + 1);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
		
	private function IsCurrentTriggered():boolean {
		if (Finished || !Config.IsServer)
			return false;
			
		return Buttons[Current].GetIsTriggered();
	}
	
	
	private function SetAllCurrentSERVERONLY(newCurrent:int) {
		if (newCurrent == Buttons.length && RestartAfterFinish)
			newCurrent = 0;
			
		NetView.RPC("RPCSetCurrent", RPCMode.AllBuffered, newCurrent);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCSetCurrent(newCurrent:int) {
		if (Current >= 0)
			Buttons[Current].enabled = false;	
			
		Current = newCurrent;
		if (Current < Buttons.length)
			Buttons[Current].enabled = true;
		else
			Finished = true;
	}
	// =============================================================================
}