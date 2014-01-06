#pragma strict

// Hannes Helmholz
//
// 


private class ClientCameraController extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	private var Config:ConfigClass;
	private var ClientCamera:Camera;
	private var ClientCameraBackground:Camera;
	private var ClientCameraEquipment:Camera;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		
		ClientCamera = transform.FindChild("Camera").GetComponent.<Camera>();
		ClientCameraBackground = transform.FindChild("CameraBackground").GetComponent.<Camera>();
		ClientCameraEquipment = transform.FindChild("CameraEquipment").GetComponent.<Camera>();
	}
	
	
	function Update() {
		if (!Config.IsServer)
			return;
		
		ClientCamera.enabled = Config.OwnClientData.ShowMainCamera;
		ClientCameraBackground.enabled = !Config.OwnClientData.ShowMainCamera;
		ClientCameraEquipment.enabled = Config.OwnClientData.ShowMainCamera;
	}
	
	
	function OnGUI() {
		GUI.skin = Config.InterfaceSkin;
		
		if (Config.IsServer && !Config.IsStandalone())
			Config.OwnClientData.ShowMainCamera = GUI.Toggle(Rect(Screen.width - 210, Screen.height - 50, 200, 20),
				Config.OwnClientData.ShowMainCamera, " render main camera");
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS STATIC --------------------------------------------------------------
	
	public static function SetAllTransforms(player:GameObject) {
		var cams:ClientCamera[] = player.transform.GetComponentsInChildren.<ClientCamera>();
		for (var cam:ClientCamera in cams)
			cam.SetOwnTransform();
	}
	// =============================================================================
}