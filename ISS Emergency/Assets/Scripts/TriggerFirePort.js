#pragma strict

// Hannes Helmholz
//
// 

private class TriggerFirePort extends TriggerAbstract {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var ObjectDescriptionExtinguish:String; // optional
	
	private var MeasurenentMeter:MeterFirePort;
	private var Extinguisher:MeterFireExtinguisher;
	private var ObjectDescriptionMeasure:String;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------	
	
	function Awake() {
		super.Awake();
		
		MeasurenentMeter = GameObject.FindObjectOfType(MeterFirePort);
		Extinguisher = GameObject.FindObjectOfType(MeterFireExtinguisher);
		
		ObjectDescriptionMeasure = ObjectDescription;
	}


	function OnMouseEnter() {
		if (MeasurenentMeter.IsEquipped()) {
			NetView.RPC("RPCSetObjectDescription", RPCMode.AllBuffered, ObjectDescriptionMeasure);
			super.OnMouseEnter();
		
		} else if (Extinguisher.IsEquipped()) {
			NetView.RPC("RPCSetObjectDescription", RPCMode.AllBuffered, ObjectDescriptionExtinguish);
			super.OnMouseEnter();
		}
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------

	protected function DoTrigger() {
		// StatusLast should not be reset on server
		// because focus still on FirePort but Highlighting should be off during measurement
		var last:boolean = StatusLast;
		NetView.RPC("RPCSetStatus", RPCMode.AllBuffered, false);
		StatusLast = last;
		
		if (MeasurenentMeter.IsEquipped())
			MeasurenentMeter.MeasureValues(gameObject.name, Trans);
		else if (Extinguisher.IsEquipped())
			Extinguisher.Extinguish(Trans);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCSetObjectDescription(text:String) {
		ObjectDescription = text;
	}
	// =============================================================================
}