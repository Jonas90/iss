//dan
//Diese Klasse wird durch den Merge des 6. Etage Projektes nicht mehr benutzt,
//da die Camera aus dem 6. Etage Projekt verwendet wird.
//Hängt noch an Config und an EquipmentAsGui


// Hannes Helmholz
//
// class for saving all relevant configuration data for a client
// every client and server owns this in config

using UnityEngine;

public class ClientCameraInfo : System.Object {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public string Ip;
	public bool ShowMainCamera;
	public bool ShowGuiText;
	public bool ShowGuiCamera;
	public float AngleOffset;
	public float ParallaxOffsetDirection;
	// =============================================================================


	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------	
	
	public ClientCameraInfo(string ip, bool showMainCamera, bool showGuiText, bool showGuiCamera, float angleOffset, float parallaxOffsetDirection)
	{
		this.Ip = ip;
		this.ShowMainCamera = showMainCamera;
		this.ShowGuiText = showGuiText;
		this.ShowGuiCamera = showGuiCamera;
		this.AngleOffset = angleOffset;
		this.ParallaxOffsetDirection = parallaxOffsetDirection;
	}
	
	
	public override string ToString() {
		string nl = System.Environment.NewLine;
		
		return "ip: " + Ip + nl +
			"showMainCamera: " + ShowMainCamera + nl +
			"showGuiText: " + ShowGuiText + nl +
			"showGuiCamera: " + ShowGuiCamera + nl +
			"angleOffset: " + AngleOffset + nl +
			"parallaxOffsetDirection: " + ParallaxOffsetDirection;
	}
	// =============================================================================
}