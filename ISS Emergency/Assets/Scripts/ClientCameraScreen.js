#pragma strict

// Hannes Helmholz
//
// represent cave screen in real world
// defindes field of view for camera
// used by ClientCamera


private class ClientCameraScreen extends System.ValueType {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public var Width:float;
	public var Height:float;
	public var Distance:float;
	// =============================================================================
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function ClientCameraScreen(width:float, height:float, distance:float) {
		this.Width = width;
		this.Height = height;
		this.Distance = distance;
	}
	
	
	public function ToString():String {
		return Width + " x " + Height + " @ " + Distance;
	}
	// =============================================================================
}