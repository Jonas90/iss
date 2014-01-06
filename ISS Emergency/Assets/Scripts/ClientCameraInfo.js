#pragma strict

// Hannes Helmholz
//
// class for saving all relevant configuration data for a client
// every client and server owns this in config


private class ClientCameraInfo extends System.Object {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public var Ip:String;
	public var ShowMainCamera:boolean;
	public var ShowGuiText:boolean;
	public var ShowGuiCamera:boolean;
	public var AngleOffset:float;
	public var ParallaxOffsetDirection:float;
	// =============================================================================


	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------	
	
	public function ClientCameraInfo(ip:String, showMainCamera:boolean, showGuiText:boolean, showGuiCamera:boolean,
									angleOffset:float, parallaxOffsetDirection:float) {
		this.Ip = ip;
		this.ShowMainCamera = showMainCamera;
		this.ShowGuiText = showGuiText;
		this.ShowGuiCamera = showGuiCamera;
		this.AngleOffset = angleOffset;
		this.ParallaxOffsetDirection = parallaxOffsetDirection;
	}
	
	
	public function ToString():String {
		var nl:String = System.Environment.NewLine;
		
		return "ip: " + Ip + nl +
			"showMainCamera: " + ShowMainCamera + nl +
			"showGuiText: " + ShowGuiText + nl +
			"showGuiCamera: " + ShowGuiCamera + nl +
			"angleOffset: " + AngleOffset + nl +
			"parallaxOffsetDirection: " + ParallaxOffsetDirection;
	}
	// =============================================================================
}