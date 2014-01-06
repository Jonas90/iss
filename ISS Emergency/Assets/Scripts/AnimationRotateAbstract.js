#pragma strict

// Hannes Helmholz
//
// (abstract) base class for animated rotations


private /*abstract*/ class AnimationRotateAbstract extends MonoBehaviour {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	protected var OnlyByServer:boolean = true;
	@SerializeField	protected var RotationAxis:Axis = Axis.Y;
	
	protected var Config:ConfigClass;
	protected var Trans:Transform;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		if (OnlyByServer) {
			try
				Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
			catch (e) {
				OnlyByServer = false;
				Config.Log(e.ToString(), true);
			}
		}
		
		Trans = transform;
	}
	
	
	function FixedUpdate() {
		if (OnlyByServer && !Config.IsServer)
			return;
			
		DoFixedUpdate();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	protected function DoFixedUpdate() {}
	// =============================================================================
}