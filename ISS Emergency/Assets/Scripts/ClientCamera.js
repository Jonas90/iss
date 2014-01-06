#pragma strict

// Hannes Helmholz
//
// running on every client and server
// set camera settings by client data from config
// calculate new client camera screen when in standalone


private class ClientCamera extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public var IndividualPlanes:boolean = false;
	
	private var Config:ConfigClass;
	private var Trans:Transform;
	private var Cam:Camera;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		Trans = transform;
		Cam = camera;
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function SetOwnTransform() {
		Trans.Rotate(Vector3(0, Config.OwnClientData.AngleOffset, 0));
	
		if (Config.IsStandalone()) {
			var res:Resolution = Screen.currentResolution;
			var h:float = Mathf.Tan((Cam.fieldOfView / 2.0) * Mathf.Deg2Rad) * Cam.nearClipPlane * 2.0;
			Config.ScreenReal = ClientCameraScreen(res.width * h / res.height, h, Cam.nearClipPlane);
		}
		
		var offset:float = (Config.ScreenParallax / 2.0) * Config.OwnClientData.ParallaxOffsetDirection;
		Trans.localPosition.x = offset;
		ClientCameraVanishingPoint.SetVanishingPoint(Cam, -offset, Config.ScreenReal);
		Config.Log("set camera " + gameObject.name + " (matrix: " + Cam.projectionMatrix + ")");
	}
	// =============================================================================
}