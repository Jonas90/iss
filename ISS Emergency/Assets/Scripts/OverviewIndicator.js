#pragma strict

// Hannes Helmholz
//
// give indicator on level overview (i.e. for player position)
//
// good looking with sphere mesh filter and diffuse material with solid color


@script RequireComponent(NetworkView) // for Transform
@script RequireComponent(MeshFilter)
@script RequireComponent(MeshRenderer)

private class OverviewIndicator extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var IndicatedObject:Transform;
	
	private var Config:ConfigClass;
	private var Trans:Transform;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		
		Trans = transform;
	}
	
	
	function Update() {
		if (!Config.IsServer)
			return;
		
		Trans.position.x = IndicatedObject.position.x;
		// Trans.position.y = not changed !!!
		Trans.position.z = IndicatedObject.position.z;
		
		Trans.rotation = IndicatedObject.rotation;
	}
	// =============================================================================
}