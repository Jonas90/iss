#pragma strict

// Hannes Helmholz
//
// manage player movement
// at beginning player flight or jump at start position
// movement will be deactivated while flight


@script RequireComponent(NetworkView) // for Transform

private class PlayerMovement extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var Speed:float = 5;
	
	private var Config:ConfigClass;
	private var CC:CharacterController;
	private var Trans:Transform;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		Trans = transform;
		
		CC = gameObject.GetComponent.<CharacterController>();
	}
	
	
	function FixedUpdate() {
		if (Input.GetAxis("Horizontal"))
			CC.Move(Trans.right * Input.GetAxis("Horizontal") * Speed * Time.deltaTime);
		
		if (Input.GetAxis("Vertical"))
			CC.Move(Trans.forward * Input.GetAxis("Vertical") * Speed * Time.deltaTime);
		
		if (Input.GetAxis("UpDown"))
			CC.Move(Trans.up * Input.GetAxis("UpDown") * Speed * Time.deltaTime);
	}
	// =============================================================================
}