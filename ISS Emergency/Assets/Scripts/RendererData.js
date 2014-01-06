#pragma strict

// Hannes Helmholz
//
// 


private class RendererData extends System.Object {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public var Rend:Renderer;
	public var MaterialOn:Material;
	public var MaterialsOriginal:Material[];
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function RendererData(rend:Renderer, materialOn:Material) {
		this.Rend = rend;
		this.MaterialsOriginal = rend.materials;
		this.MaterialOn = materialOn;
	}
	
	
	public function SetMaterials(status:boolean) {
		var mats:Material[] = new Material[MaterialsOriginal.Length];
		for (var i:int = 0; i < MaterialsOriginal.Length; i++)
			mats[i] = (status) ? MaterialOn : MaterialsOriginal[i];
		
		Rend.materials = mats;
	}
	
	
	public function SetEnabled(status:boolean) {
		Rend.enabled = status;
	}
	// =============================================================================
}