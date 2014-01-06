#pragma strict

// Hannes Helmholz
//
// 


@script ExecuteInEditMode()

private class SkyboxController extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField private var FreezeHorizonHeight:boolean = true;
	@SerializeField	private var MainCamerasNearClipPlane:float = 0.35;
	@SerializeField	private var MainCamerasFarClipPlane:float = 1000;
	@SerializeField	private var MainCamerasFieldOfView:float = 60;
	
	private var Player:Transform;
	private var PlayerCC:CharacterController;
	private var Trans:Transform;
	private var Cams:Camera[];
	private var CamsIndividual:boolean[];
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		var object:GameObject = GameObject.FindWithTag("Player");
		if (object) {
			Player = object.transform;
			PlayerCC = object.GetComponent.<CharacterController>();
		}
			
		var objects:GameObject[] = GameObject.FindGameObjectsWithTag("MainCamera");
		if (objects) { 
			Cams = new Camera[objects.Length];
			CamsIndividual = new boolean[objects.Length];
			
			for (var i:int = 0; i < objects.Length; i++) {
				Cams[i] = objects[i].GetComponent.<Camera>();
				
				var client:ClientCamera = objects[i].GetComponent.<ClientCamera>();
				CamsIndividual[i] = client ? client.IndividualPlanes : false;
			}
		}
	
		Trans = transform;
	}
	
	
	function Update() {
		if (Player) {
			Trans.position.x = Player.position.x;
			if (!FreezeHorizonHeight)
				Trans.position.y = Player.position.y;
			Trans.position.z = Player.position.z;
			Trans.localScale = Vector3(MainCamerasFarClipPlane * 2, MainCamerasFarClipPlane * 2, MainCamerasFarClipPlane * 2);
			
			if (PlayerCC) { // deactivated in cave on clients
				var res:Resolution = Screen.currentResolution;
				var h:float = Mathf.Tan((MainCamerasFieldOfView / 2.0) * Mathf.Deg2Rad) * MainCamerasNearClipPlane * 2.0;
				var w:float = res.width * h / res.height;
				PlayerCC.radius = Mathf.Sqrt(Mathf.Pow(w / 2.0, 2) + Mathf.Pow(MainCamerasNearClipPlane, 2));
			}
		}
		
		if (Cams)
			for (var i:int = 0; i < Cams.length; i++) {
				Cams[i].fieldOfView = MainCamerasFieldOfView;
				
				if (!CamsIndividual[i]) {
					Cams[i].nearClipPlane = MainCamerasNearClipPlane;
					Cams[i].farClipPlane = MainCamerasFarClipPlane;
				}
				
			}
	}
	// =============================================================================
}