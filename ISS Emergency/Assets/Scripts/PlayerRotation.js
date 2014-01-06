#pragma strict

// Hannes Helmholz
//
//


private class PlayerRotation extends MouseLook {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@HideInInspector public var TemporaryDisable:boolean = false;
	
	private var Config:ConfigClass;
	private var DoRotation:boolean = false;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
	}
	
	
	function Update() {
		if (TemporaryDisable)
			return;
	
		if (Config.IsStandalone() && Input.GetMouseButtonDown(1)) { // right mouse key
			DoRotation = !DoRotation;
			Screen.lockCursor = DoRotation;
		}
		
		if (DoRotation) {
			axisX = (Config.InputDevice == Config.Device.Keyboard) ? "Mouse X" : "LookHorizontal";
			axisY = (Config.InputDevice == Config.Device.Keyboard) ? "Mouse Y" : "LookVertical";
			
			super.Update();
		}
	}
	
	
	function OnEnable() {
		DoRotation = true;
		
		if (Config.IsStandalone())
			Screen.lockCursor = true;
	}
	
	
	function OnDisable() {
		Screen.lockCursor = false;
	}
	// =============================================================================
}