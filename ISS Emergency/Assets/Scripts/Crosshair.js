#pragma strict

// Hannes Helmholz
//
// showing crosshair gui in world space
// 
// raycast stops on all colliders
// -> DISABLE "Raycast Hit Triggers" IN Edit/Project Setting/Physics


@script RequireComponent(NetworkView) // for RPC

private class Crosshair extends MonoBehaviour {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField private var Tex:Texture2D;
	@SerializeField private var TextureBaseSize:Vector2 = Vector2(130, 130);
	@SerializeField private var TextureMinSize:Vector2 = Vector2(25, 25);
	@SerializeField private var MovementSmooth:float = 5;
	@SerializeField private var Cam:Camera;
	
	
	private var Config:ConfigClass;
	private var CamTransform:Transform;
	private var NetView:NetworkView;
	private var WorldPosition:Vector3;
	private var Position:Vector3;
	private var Size:Vector2;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		
		CamTransform = Cam.transform;
		NetView = gameObject.networkView;
	}
	
	
	function Update() {
		if (!Config.IsServer)
			return;

		var ray:Ray = Cam.ScreenPointToRay(Input.mousePosition);
		var hit:RaycastHit;
		if (Physics.Raycast(ray, hit))
			NetView.RPC("RPCUpdatePosition", RPCMode.AllBuffered, hit.point);
		else // point in infinity under cursor
			NetView.RPC("RPCUpdatePosition", RPCMode.AllBuffered, ray.GetPoint(Cam.farClipPlane));
	}
	
	
	function OnGUI() {
		if (Config.OwnClientData.AngleOffset != 0)
			return;
			
		if ((Config.IsServer && !Config.IsStandalone()) || (Config.IsStandalone() && !Screen.lockCursor))
			return;
			
		Position = Vector3.Lerp(Position, Cam.WorldToScreenPoint(WorldPosition), Time.deltaTime * MovementSmooth);
		
		var newSize = TextureBaseSize / (CamTransform.position - WorldPosition).magnitude;
		if (newSize.x < TextureMinSize.x)
			newSize.x = TextureMinSize.x;
		if (newSize.y < TextureMinSize.y)
			Size.y = TextureMinSize.y;	
		Size = Vector2.Lerp(Size, newSize, Time.deltaTime * MovementSmooth);
		
		GUI.DrawTexture(Rect(Position.x - (Size.x / 2), Position.y - (Size.y / 2), Size.x, Size.y), Tex, ScaleMode.ScaleToFit);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCUpdatePosition(worldPosition:Vector3) {
		WorldPosition = worldPosition;
	}
	// =============================================================================
}
