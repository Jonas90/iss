#pragma strict

// Hannes Helmholz
//
// 


@script RequireComponent(NetworkView) // for RPC

private class EquipmentAsGui extends Equipment {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField private var Textures:CaveTextures;
	
	private var NetView:NetworkView;
	private var GuiTex:GUITexture;
	private var FirstTaken:boolean = false;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		super.Start();
		
		NetView = networkView;
		GuiTex = guiTexture;
		
		GuiTex.texture = (Config.IsServer) ? Textures.Standalone : GetTextureAsClient();
		if (AvailableAtStart)
			for (var tweenTarget:Object in TweenTargets)
				HOTween.PlayBackwards(tweenTarget);
	}
	
	
	function Update() {
		if (!Config.IsServer)
			return;
			
		if (Available && TriggerManual) {
			NetView.RPC("RPCTrigger", RPCMode.AllBuffered);
			TriggerManual = false;
		}
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function TriggerStored() {
		if (FirstTaken)
			return;
	
		FirstTaken = true;
		super.TriggerStored();
	}


	public function IsEquipped():boolean {
		return Available;
	}
	
	
	private function GetTextureAsClient():Texture2D {
		var angles:ArrayList = GetAllCameraAngles();
		var indexDiff:int = (angles.Count / 2) - angles.IndexOf(Config.OwnClientData.AngleOffset);
		
		if (indexDiff == 0)
			return Textures.Front;
		else if (indexDiff == 1)
			return Textures.Left;
		else if (indexDiff == -1)
			return Textures.Right;
		else
			return Textures.Outer;
	}
	
	
	private function GetAllCameraAngles():ArrayList {
		var angles:ArrayList = new ArrayList();
		for (var i:int = 1; i < Config.ClientData.length; i++) {
			var angle:float = (Config.ClientData[i] as ClientCameraInfo).AngleOffset;
			if (!angles.Contains(angle))
				angles.Add(angle);
		}
		
		angles.Sort();
		return angles;
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCTrigger() {
		Trigger();
	}
	// =============================================================================
}



private class CaveTextures extends System.Object {
	public var Standalone:Texture2D;
	public var Left:Texture2D;
	public var Front:Texture2D;
	public var Right:Texture2D;
	public var Outer:Texture2D;
}