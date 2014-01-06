#pragma strict

// Hannes Helmholzd
//
// 


import Holoville.HOTween;

@script RequireComponent(NetworkView) // for Transform
@script RequireComponent(LayerSetterElement)

private class TriggerEquipment extends TriggerAbstract {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField private var LinkedEquipment:Equipment;
	@SerializeField private var TakeAnimationLength:float = 2; // optional
	
	private var PlayerMov:PlayerMovement;
	private var PlayerRot:PlayerRotation;
	private var Layer:LayerSetterElement;
	private var EquipManager:EquipmentManager;
	// =============================================================================



	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		super.Awake();
		
		PlayerMov = Player.GetComponent.<PlayerMovement>();
		PlayerRot = Player.GetComponent.<PlayerRotation>();
		Layer = gameObject.GetComponent.<LayerSetterElement>();
		EquipManager = GameObject.FindObjectOfType(EquipmentManager);
	}
	
	
	function OnMouseEnter() {
		var atMercy:EquipmentAtMercy = LinkedEquipment as EquipmentAtMercy;
		if (!LinkedEquipment.IsAvailable() && !(atMercy && atMercy.IsPotentionallyAvailable()))
			super.OnMouseEnter();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------

	protected function DoTrigger() {
		for (var rend:RendererData in Renderers)
			rend.SetMaterials(false);
			
		StartCoroutine(YieldDoTrigger());
	}
	
	
	// this design is stupid, but calling "yield StartCoroutine(Animate())" in DoTrigger() doesn't work
	private function YieldDoTrigger() {
		if (TakeAnimationLength > 0)
			yield StartCoroutine(Animate());
		
		EquipManager.Take(LinkedEquipment);
		NetView.RPC("RPCDoTrigger", RPCMode.AllBuffered);
	}
	

	private function SetPlayerMotion(status:boolean) {
		PlayerMov.enabled = status;
		PlayerRot.TemporaryDisable = !status;
	}

	
	private function Animate() {
		if (Layer.OldLayer != Layer.OtherLayer)
			LayerSetter.SetAllLayerRecursively(gameObject, Layer.OtherLayer);
	
		SetPlayerMotion(false);
		
		var tweenTarget:Transform = LinkedEquipment.transform;
		if (LinkedEquipment as EquipmentAsGui)
			tweenTarget = Player.transform;
			
		var parms:TweenParms = TweenParms();
		parms.Prop("position", Plugins.Core.PlugVector3(tweenTarget.position));
		parms.Prop("rotation", Plugins.PlugQuaternion(tweenTarget.rotation));
		parms.Prop("localScale", Plugins.Core.PlugVector3(tweenTarget.localScale));
		parms.Ease(EaseType.EaseInOutSine);
		
		HOTween.To(Trans, TakeAnimationLength, parms);
		yield WaitForSeconds(TakeAnimationLength);
		
		SetPlayerMotion(true);
		
		LayerSetter.SetAllLayerRecursively(gameObject, Layer.OldLayer);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCDoTrigger() {
		gameObject.SetActive(false);
	}
	// =============================================================================
}