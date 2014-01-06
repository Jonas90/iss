#pragma strict

// Hannes Helmholz
//
//


@script RequireComponent(Collider) // for trigger of mouse
@script RequireComponent(NetworkView) // for RPC

private /*abstract*/ class TriggerAbstract extends FunkyGlowingThingsElement {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField protected var HoverText:HoveringText; // optional
	@SerializeField protected var HoverTextOffset:Vector3 = Vector3(0, 0, -0.15); // optional
	@SerializeField	protected var Button:InteractionKey; // optional
	@SerializeField	protected var ObjectDescription:String; // optional
	
	protected var Coll:Collider;
	// ============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		super.Awake();
		Button.Initialize();
		
		Coll = collider;
	}
	
	
	function Update() {
		super.Update();
		
		if (!Config.IsServer)
			return;
			
		if (GetIsTriggered())
			DoTrigger();
	}
	
	
	function OnDisable() {
		super.RPCSetStatus(false);
		
		if (HoverText)
			HoverText.Hide();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function GetIsTriggered():boolean {
		return super.GetIsTriggered() && Button.GetButtonDown();
	}
	
	
	// for override
	protected function DoTrigger() {
		NetView.RPC("RPCDoTrigger", RPCMode.AllBuffered);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCSetStatus(status:boolean) {
		super.RPCSetStatus(status);
	
		if (HoverText) {
			if (status) {
				var text:String = "";
				if (Button.GetCurrentButton().Length != 0)
					text += /*"System.Environment.NewLine +*/ "press button " + Button.GetCurrentButton(" or ");
				if (ObjectDescription.Length != 0)
					text += System.Environment.NewLine + ObjectDescription;
				
				HoverText.ViewOnTarget(Trans, HoverTextOffset, text);
			} else
				HoverText.Hide();
		}
	}
	
	
	@RPC
	// for override
	function RPCDoTrigger() {}
	// =============================================================================
}