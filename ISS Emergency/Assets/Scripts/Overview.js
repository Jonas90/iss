#pragma strict

// Hannes Helmholz
//
// give level overview as a map
// 
// needs camera, i.e. ortographic
// camera aspect ratio can be set by normalized view port rect
// ortographic camera can be set on discrete height with near clipping plane at 0 to cut through roofs
//
// screen position in viewport coordinates [0..l] 


@script RequireComponent(GuiScreenPosition)

private class Overview extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var Cam:Camera;
	@SerializeField	private var Background:GUITexture; // optional
	
	private var Config:ConfigClass;
	
	private var ScreenPosition:GuiScreenPosition;
	private var BackgroundTrans:Transform;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		
		ScreenPosition = gameObject.GetComponent.<GuiScreenPosition>();
		
		if (Background)
			BackgroundTrans = Background.transform;
		
		if (Cam.aspect >= 1)
			ScreenPosition.SetCameraAspect(Cam.rect.width / Cam.rect.height);
		else 
			ScreenPosition.SetCameraAspect(Cam.rect.height / Cam.rect.width);
	}
	
	
	function Update() {
		if (!Config.OwnClientData.ShowGuiCamera || (Config.IsStandalone() && !Screen.lockCursor)) {
			Cam.enabled = false;
			if (Background)
				Background.enabled = false;
			
			return;
		}
		
		Cam.enabled = true;
		Cam.rect = Rect(0, 0, 1, 1);
		
		var xy:Vector3 = Cam.ViewportToScreenPoint(Vector3(ScreenPosition.X(), ScreenPosition.Y(), 0));
		var wh:Vector3 = Cam.ViewportToScreenPoint(Vector3(ScreenPosition.Width(), ScreenPosition.Height(), 0));
		var screenPixels:Rect = Rect(xy.x, xy.y, wh.x, wh.y);
		Cam.pixelRect = screenPixels;
	
		if (Background) {
			Background.enabled = true;
			BackgroundTrans.localScale = Vector3.zero; 
			Background.pixelInset = screenPixels;
		}
	}
	// =============================================================================
}